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

namespace Intergration;

[TestFixture]
public class AddTaskCommandTests
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
    public void Setup()
    {
        _checklistRepo = new ChecklistRepo(
                            TestFactory.GetEventStore(),
                            TestFactory.GetChecklistStateStore());

        _userRepo = new UserRepo(
                            TestFactory.GetEventStore(),
                            TestFactory.GetUserStateStore());

        var identityMock = new Mock<IIdentityProvider>();
        identityMock.Setup(i => i.GetCurrentUserId()).Returns(CurrentUserId);
        identityMock.Setup(i => i.GetCurrentUserName()).Returns(CurrentUserName);
        _identityMock = identityMock.Object;

        var domainEventDispatcherMock = new Mock<IDomainEventDispatcher>();
        domainEventDispatcherMock.Setup(i => i.Dispatch(
                    It.IsAny<IDomainEvent>(),
                    It.IsAny<CancellationToken>()));
        _eventDispatcherMock = domainEventDispatcherMock.Object;

        _unitOfWork = new SQLiteUnitOfWork(
                            new Logger<SQLiteUnitOfWork>(new LoggerFactory()),
                            TestFactory.GetEventsContext(),
                            TestFactory.GetEntitiesContext(),
                            _eventDispatcherMock);

        var entitiesContext = TestFactory.GetEntitiesContext();
        entitiesContext.RemoveRange(entitiesContext.Users);
        entitiesContext.RemoveRange(entitiesContext.Checklists);
        entitiesContext.SaveChanges();

        var eventsContext = TestFactory.GetEventsContext();
        eventsContext.RemoveRange(eventsContext.Events);
        eventsContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        TestFactory.Reset();
    }


    [Test]
    public async Task AddTaskCommandHandler_Success()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var sut = new AddTaskCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, CurrentUserId);
        await _checklistRepo.Save(checklist, default);
        await _unitOfWork.Commit();

        var command = new AddTaskCommand(
                checklistId,
                _fix.Create<Name>(),
                _fix.Create<TaskType>(),
                _fix.Create<Schedule>(),
                _fix.Create<string>());

        var taskId = await sut.Handle(command, default);

        var storedChecklist = await _checklistRepo.GetById(checklistId, default);
        Assert.That(storedChecklist, Is.Not.Null);

        var task = storedChecklist.Tasks[0];
        Assert.That(task, Is.Not.Null);
        Assert.That(task.Id, Is.EqualTo(taskId));
        Assert.That(task.ChecklistId, Is.EqualTo(checklistId));
    }
}

