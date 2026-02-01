using Application.Interfaces.Events;
using AutoFixture;
using Domain.Entities.Schedules;
using Domain.Entities.Schedules.Events;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos;
using TestHelperFactory;

namespace InfrastructureTests.RepoTests;


[TestFixture]
public class ChecklistRepoTests // TODO:
{
    private readonly Fixture _fix = TestFactory.GetFixture();
    private readonly IEventStore store = TestFactory.GetEventStore();
    private readonly ITrackedEntities _trackedEntities = TestFactory.GetTrackedEntities();


    [Test]
    public async Task SaveAsync_Success()
    {
        // Arrange
        var checklist = new Checklist();
        var checklistId = _fix.Create<ChecklistId>();
        var userId = _fix.Create<UserId>();
        checklist.Create(checklistId, userId);

        var sut = new ChecklistRepo(store, _trackedEntities);

        // Act
        await sut.SaveAsync(checklist, default);


        // Assert
        var res = await store.LoadAsync(checklist.Id, default);

        var trackedEntities = _trackedEntities.GetDictionary();
        var checklistEntities = trackedEntities[typeof(Checklist)];

        Assert.That(checklistEntities, Does.Contain(checklist));
        // Assert.That(res, Is.EqualTo(checklist));
    }

}

