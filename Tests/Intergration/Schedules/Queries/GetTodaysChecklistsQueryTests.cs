using System.Linq;
using System.Text.Json;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Application.UseCases.Schedules;
using Application.UseCases.Schedules.Queries;
using AutoFixture;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Repos;
using Moq;
using TestHelperFactory;

namespace Intergration.Schedules.Queries;

[TestFixture]
public class GetTodaysChecklistsQueryTests
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
    public void TearDown() { TestFactory.Reset(); }


    [Test]
    public async Task GetTodaysChecklistsQuery_Success()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var sut = new GetTodaysChecklistsQueryHandler(
                _identityMock,
                _checklistRepo,
                _userRepo);

        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, CurrentUserId);

        await _checklistRepo.Save(checklist, default);
        await _unitOfWork.Commit();

        var result = (await sut.Handle(new GetTodaysChecklistsQuery(), default)).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));

        Console.WriteLine(JsonSerializer.Serialize(checklist.State));
        Console.WriteLine(JsonSerializer.Serialize(result[0]));
    }

    [Test]
    public async Task GetTodaysChecklistsQuery_FiltersByDate()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var otherDate = today.AddDays(-7);

        var todayChecklist1 = CreateChecklistWithDate(today, CurrentUserId);
        var todayChecklist2 = CreateChecklistWithDate(today, CurrentUserId);
        var otherDateChecklist1 = CreateChecklistWithDate(otherDate, CurrentUserId);
        var otherDateChecklist2 = CreateChecklistWithDate(otherDate, CurrentUserId);

        await _checklistRepo.Save(todayChecklist1, default);
        await _checklistRepo.Save(todayChecklist2, default);
        await _checklistRepo.Save(otherDateChecklist1, default);
        await _checklistRepo.Save(otherDateChecklist2, default);
        await _unitOfWork.Commit();

        var sut = new GetTodaysChecklistsQueryHandler(
                _identityMock,
                _checklistRepo,
                _userRepo);

        var result = (await sut.Handle(new GetTodaysChecklistsQuery(), default)).ToList();

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(c => c.Statistics.GetDate() == today), Is.True);
    }

    private Checklist CreateChecklistWithDate(DateOnly date, UserId userId)
    {
        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, userId);

        // Use reflection to set the private Statistics property
        var stateType = typeof(ChecklistState);
        var timestamp = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var newStatistics = new Statistics(timestamp);

        // Get the private setter
        var statisticsSetter = stateType.GetProperty("Statistics", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        statisticsSetter!.SetValue(checklist.State, newStatistics);

        return checklist;
    }
}
