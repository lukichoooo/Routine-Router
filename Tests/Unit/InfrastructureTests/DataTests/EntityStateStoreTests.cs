using AutoFixture;
using Domain.Common.ValueObjects;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Contexts;
using TestHelperFactory;

namespace InfrastructureTests.DataTests;


[TestFixture]
public class EntityStateStoreTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly EntitiesContext _context = TestFactory.GetEntitiesContext();

    [SetUp]
    public void Setup()
    {
        _context.Users.RemoveRange(_context.Users);
        _context.Checklists.RemoveRange(_context.Checklists);
        _context.SaveChanges();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        TestFactory.Reset();
    }


    private List<IAggregateRoot> GetTrackedEntityStateOwners()
            => _context.ChangeTracker
            .Entries<IAggregateRootState>()
            .Select(e => e.Entity.Owner)
            .Where(e => e != null)
            .ToList()!;

    // User

    [Test]
    public async Task Get_User_Success()
    {
        // Arrange
        var user = new User();
        var userId = _fix.Create<UserId>();
        var name = _fix.Create<Name>();
        var passHash = _fix.Create<PasswordHash>();
        user.Create(userId, name, passHash);
        await _context.Users.AddAsync(user.State);
        _context.SaveChanges();

        var sut = new SQLiteStateStore<UserState, UserId>(_context);

        // Act
        var res = await sut.Get(user.Id, default);
        var currentlyTrackedEntities = GetTrackedEntityStateOwners();

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(user));
        Assert.That(res, Is.EqualTo(user.State));
    }

    [Test]
    public async Task Save_User_Success()
    {
        // Arrange
        var user = new User();
        var userId = _fix.Create<UserId>();
        var name = _fix.Create<Name>();
        var passHash = _fix.Create<PasswordHash>();
        user.Create(userId, name, passHash);

        var sut = new SQLiteStateStore<UserState, UserId>(_context);

        // Act
        await sut.Save(user.State, default);
        var currentlyTrackedEntities = GetTrackedEntityStateOwners();

        _context.SaveChanges();

        var res = await _context.Users.FindAsync(userId);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(user));
        Assert.That(res, Is.EqualTo(user.State));
    }

    [Test]
    public async Task Save_Update_User_Success()
    {
        // Arrange
        var sut = new SQLiteStateStore<UserState, UserId>(_context);

        var user = new User();
        var userId = _fix.Create<UserId>();
        var name = _fix.Create<Name>();
        var passHash = _fix.Create<PasswordHash>();
        user.Create(userId, name, passHash);
        await sut.Save(user.State, default);
        _context.SaveChanges();

        var storedUserState = await _context.Users.FindAsync(userId);
        Assert.That(storedUserState, Is.Not.Null);
        var storedUser = new User(storedUserState);
        storedUser.Update(_fix.Create<Name>(), _fix.Create<PasswordHash>());

        // Act
        await sut.Save(storedUser.State, default);
        var currentlyTrackedEntities = GetTrackedEntityStateOwners();
        _context.SaveChanges();

        var res = await _context.Users.FindAsync(userId);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(user));
        Assert.That(res, Is.EqualTo(storedUser.State));
    }



    // Checklist

    [Test]
    public async Task Get_Checklist_Success()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);
        await _context.Checklists.AddAsync(checklist.State);
        _context.SaveChanges();

        var sut = new SQLiteStateStore<ChecklistState, ChecklistId>(_context);
        var currentlyTrackedEntities = GetTrackedEntityStateOwners();

        // Act
        var res = await sut.Get(checklist.Id, default);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(checklist));
        Assert.That(res, Is.EqualTo(checklist.State));
    }


    [Test]
    public async Task Save_Checklist_Success()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);

        var sut = new SQLiteStateStore<ChecklistState, ChecklistId>(_context);

        // Act
        await sut.Save(checklist.State, default);
        var currentlyTrackedEntities = GetTrackedEntityStateOwners();
        _context.SaveChanges();

        var res = await _context.Checklists.FindAsync(checklistId);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(checklist));
        Assert.That(res, Is.EqualTo(checklist.State));
    }


    [Test]
    public async Task Save_Update_Checklist_Success()
    {
        // Arrange
        var sut = new SQLiteStateStore<ChecklistState, ChecklistId>(_context);

        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);
        await sut.Save(checklist.State, default);
        _context.SaveChanges();

        var storedChecklistState = await _context.Checklists.FindAsync(checklistId);
        Assert.That(storedChecklistState, Is.Not.Null);
        var storedChecklist = new Checklist(storedChecklistState);

        storedChecklist.AddTask(
                _fix.Create<Name>(),
                _fix.Create<TaskType>(),
                _fix.Create<Schedule>(),
                _fix.Create<string>());

        // Act
        await sut.Save(storedChecklist.State, default);
        var currentlyTrackedEntities = GetTrackedEntityStateOwners();
        _context.SaveChanges();

        var res = await _context.Checklists.FindAsync(checklistId);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(checklist));
        Assert.That(res, Is.EqualTo(storedChecklist.State));
    }

}
