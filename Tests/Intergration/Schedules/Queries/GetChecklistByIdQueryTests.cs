using System.Text.Json;
using Application.Interfaces.Data;
using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Application.UseCases.Schedules;
using Application.UseCases.Schedules.Queries;
using AutoFixture;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Repos;
using Moq;
using TestHelperFactory;

namespace Intergration.Schedules.Queries;

[TestFixture]
public class GetChecklistByIdQueryTests
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
    public async Task GetChecklistByIdQuery_Success()
    {
        var user = new User();
        user.Create(CurrentUserId, new(CurrentUserName), _fix.Create<PasswordHash>());
        await _userRepo.Save(user, default);
        await _unitOfWork.Commit();

        var sut = new GetChecklistByIdQueryHandler(
                _identityMock,
                _checklistRepo,
                _userRepo);

        var checklist = new Checklist();
        var checklistId = new ChecklistId(Guid.NewGuid());
        checklist.Create(checklistId, CurrentUserId);
        await _checklistRepo.Save(checklist, default);
        await _unitOfWork.Commit();
        Console.WriteLine(JsonSerializer.Serialize(checklist.State));

        var command = new GetChecklistByIdQuery(checklistId);

        var result = await sut.Handle(command, default);
        Console.WriteLine(JsonSerializer.Serialize(result));

        Assert.That(result, Is.Not.Null);
        Assert.That(JsonSerializer.Serialize(checklist.State),
                Is.EqualTo(JsonSerializer.Serialize(result)));
    }

}
