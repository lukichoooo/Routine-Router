using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Application.UseCases.Schedules;
using Application.UseCases.Schedules.Commands;
using AutoFixture;
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
public class CreateChecklistCommandTests
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
    public async Task CreateChecklistCommandHandlerTest_Success()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        // Arrange
        var sut = new CreateChecklistCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var command = new CreateChecklistCommand();

        var checklistId = await sut.Handle(command, default);

        // Assert
        Assert.That(checklistId, Is.Not.Null);

        var stored = await _checklistRepo.GetById(checklistId, default);
        Assert.That(stored, Is.Not.Null);
        Assert.That(stored.Id, Is.EqualTo(checklistId));
        Assert.That(stored.Version, Is.EqualTo(1));
    }


    [Test]
    public async Task CreateChecklistCommandHandlerTest_UserNotFound_Throws()
    {
        // Arrange
        var sut = new CreateChecklistCommandHandler(
                _identityMock,
                _checklistRepo,
                _userRepo,
                _unitOfWork);

        var command = new CreateChecklistCommand();

        Assert.ThrowsAsync<ApplicationArgumentException>
            (async () => await sut.Handle(command, default));
    }
}

