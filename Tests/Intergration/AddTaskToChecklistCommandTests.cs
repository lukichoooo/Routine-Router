using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Application.UseCases.Schedules;
using AutoFixture;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence;
using Infrastructure.Repos;
using Microsoft.Extensions.Logging;
using Moq;
using TestHelperFactory;

namespace Intergration;

[TestFixture]
public class AddTaskToChecklistCommandTests
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
    public async Task CreateChecklistCommandHandlerTest_Success()
    {
    }
}

