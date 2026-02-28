using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Application.UseCases.Schedules;
using Application.UseCases.Schedules.Commands;
using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence;
using Infrastructure.Repos;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelperFactory;

namespace Intergration.Schedules.Commands;

[TestFixture]
public class RemoveTaskCommandTests
{
    public Fixture _fix = TestFactory.GetFixture();


    private IIdentityProvider _identityMock = null!;
    private IDomainEventDispatcher _eventDispatcherMock = null!;
    private IUnitOfWork _unitOfWork = null!;
    private IChecklistRepo _checklistRepo = null!;
    private IUserRepo _userRepo = null!;

    private const string CurrentUserName = "luka";
    private readonly UserId CurrentUserId = new(Guid.NewGuid());

    [SetUp]
    public async Task SetupAsync()
    {
        _checklistRepo = new ChecklistRepo(
                            await TestFactory.GetEventStoreAsync(),
                            await TestFactory.GetChecklistStateStoreAsync());

        _userRepo = new UserRepo(
                            await TestFactory.GetEventStoreAsync(),
                            await TestFactory.GetUserStateStoreAsync());

        var identityMock = new Mock<IIdentityProvider>();
        identityMock.Setup(i => i.GetCurrentUserId()).Returns(CurrentUserId);
        identityMock.Setup(i => i.GetCurrentUserName()).Returns(CurrentUserName);
        _identityMock = identityMock.Object;

        var domainEventDispatcherMock = new Mock<IDomainEventDispatcher>();
        domainEventDispatcherMock.Setup(i => i.Dispatch(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()));
        _eventDispatcherMock = domainEventDispatcherMock.Object;

        _unitOfWork = await TestFactory.GetTestUnitOfWorkAsync(_eventDispatcherMock);

        var stateContext = await TestFactory.GetStateContextAsync();
        stateContext.RemoveRange(stateContext.Users);
        stateContext.RemoveRange(stateContext.Checklists);
        stateContext.SaveChanges();

        var eventContext = await TestFactory.GetEventContextAsync();
        eventContext.RemoveRange(eventContext.Events);
        eventContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        TestFactory.Reset();
    }


    [Test]
    public async Task RemoveTaskCommandHandler_Success()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, CurrentUserId);

        var taskId = checklist.AddTask(
            _fix.Create<Name>(),
            _fix.Create<TaskType>(),
            _fix.Create<Schedule>(),
            _fix.Create<string>());

        await _checklistRepo.Save(checklist, default);
        await _unitOfWork.Commit();

        var sut = new RemoveTaskCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var command = new RemoveTaskCommand(
                checklistId,
                taskId);

        await sut.Handle(command, default);

        var storedChecklist = await _checklistRepo.GetById(checklistId, default);
        Assert.That(storedChecklist, Is.Not.Null);
        Assert.That(storedChecklist.Tasks.FirstOrDefault(t => t.Id == taskId), Is.Null);
    }

    [Test]
    public async Task RemoveTaskCommandHandler_FailWhenUserDoesNotExist()
    {
        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, CurrentUserId);

        var taskId = checklist.AddTask(
            _fix.Create<Name>(),
            _fix.Create<TaskType>(),
            _fix.Create<Schedule>(),
            _fix.Create<string>());

        await _checklistRepo.Save(checklist, default);
        await _unitOfWork.Commit();

        var sut = new RemoveTaskCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var command = new RemoveTaskCommand(
                checklistId,
                taskId);

        Assert.ThrowsAsync<ApplicationArgumentException>(
            async () => await sut.Handle(command, default));
    }



    [Test]
    public async Task RemoveTaskCommandHandler_Fail_WhenUserDoesNotOwnChecklist()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var otherUserId = new UserId(Guid.NewGuid());
        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, otherUserId);

        var taskId = checklist.AddTask(
            _fix.Create<Name>(),
            _fix.Create<TaskType>(),
            _fix.Create<Schedule>(),
            _fix.Create<string>());

        checklist.RemoveTask(taskId);

        await _checklistRepo.Save(checklist, default);
        await _unitOfWork.Commit();

        var sut = new RemoveTaskCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var command = new RemoveTaskCommand(
                checklistId,
                taskId);

        Assert.ThrowsAsync<ApplicationArgumentException>(
            async () => await sut.Handle(command, default));
    }


    [Test]
    public async Task RemoveTaskCommandHandler_Fail_WhenChecklistDoesNotExist()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var sut = new RemoveTaskCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var nonExistentChecklistId = new ChecklistId(Guid.NewGuid());
        var taskId = new TaskId(Guid.NewGuid());

        var command = new RemoveTaskCommand(
                nonExistentChecklistId,
                taskId);

        Assert.ThrowsAsync<ApplicationArgumentException>(
            async () => await sut.Handle(command, default));
    }

}


