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
public class EntityStoreTests
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly EntitiesContext _context = TestFactory.GetEntitiesContext();

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        await _context.DisposeAsync();
    }

    // TODO: track the entities instead of states
    private List<IAggregateRoot> GetTrackedEntities()
            => _context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .ToList();

    // User

    [Test]
    public async Task GetAsync_User_Success()
    {
        // Arrange
        var user = new User();
        var userId = _fix.Create<UserId>();
        var name = _fix.Create<Name>();
        var passHash = _fix.Create<PasswordHash>();
        user.Create(userId, name, passHash);
        await _context.Users.AddAsync(user.State);
        _context.SaveChanges();

        var sut = new SQLiteUserStateStore(_context);

        // Act
        var res = await sut.GetAsync(user.Id, default);
        var currentlyTrackedEntities = GetTrackedEntities();

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Not.Contain(user.State));
        Assert.That(res, Is.EqualTo(user.State));
    }

    [Test]
    public async Task AddAsync_User_Success()
    {
        // Arrange
        var user = new User();
        var userId = _fix.Create<UserId>();
        var name = _fix.Create<Name>();
        var passHash = _fix.Create<PasswordHash>();
        user.Create(userId, name, passHash);

        var sut = new SQLiteUserStateStore(_context);

        // Act
        await sut.AddAsync(user.State, default);
        var currentlyTrackedEntities = GetTrackedEntities();

        _context.SaveChanges();

        var res = await _context.Users.FindAsync(userId);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(user.State));
        Assert.That(res, Is.EqualTo(user.State));
    }


    // [Test]
    // public async Task Update_User_Success()
    // {
    //     // Arrange
    //     var user = new User();
    //     var userId = _fix.Create<UserId>();
    //     var name = _fix.Create<Name>();
    //     var passHash = _fix.Create<PasswordHash>();
    //     user.Create(userId, name, passHash);
    //     await _context.Users.AddAsync(user.State);
    //     _context.SaveChanges();
    //
    //     var storedState = await _context.Users.FindAsync(userId);
    //     var updatedUser = new User(storedState!);
    //     var updatedName = _fix.Create<Name>();
    //     var updatedPassword = _fix.Create<PasswordHash>();
    //     updatedUser.Update(updatedName, updatedPassword);
    //     _context.SaveChanges();
    //
    //     // Act
    //
    //     var res = await _context.Users.FindAsync(userId);
    //
    //     // Assert
    //     var trackedEntities = GetTrackedEntities();
    //     Assert.That(trackedEntities, Does.Contain(user));
    //     Assert.That(res, Is.EqualTo(user.State));
    // }
    //



    // Checklist

    [Test]
    public async Task GetAsync_Checklist_Success()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);
        await _context.Checklists.AddAsync(checklist.State);
        _context.SaveChanges();

        var sut = new SQLiteChecklistStateStore(_context);
        var currentlyTrackedEntities = GetTrackedEntities();

        // Act
        var res = await sut.GetAsync(checklist.Id, default);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Not.Contain(checklist.State));
        Assert.That(res, Is.EqualTo(checklist.State));
    }


    [Test]
    public async Task AddAsync_Checklist_Success()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);

        var sut = new SQLiteChecklistStateStore(_context);

        // Act
        await sut.AddAsync(checklist.State, default);
        var currentlyTrackedEntities = GetTrackedEntities();
        _context.SaveChanges();

        var res = await _context.Checklists.FindAsync(checklistId);

        // Assert
        Assert.That(currentlyTrackedEntities, Does.Contain(checklist.State));
        Assert.That(res, Is.EqualTo(checklist.State));
    }
}
