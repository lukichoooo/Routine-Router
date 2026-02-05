using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos.Common;

namespace Infrastructure.Repos;


public class UserRepo : BaseRepository<User, UserId, UserState>, IUserRepo
{
    private readonly IEventStore _eventStore;

    public UserRepo(
            IEventStore eventStore,
            IEntityStateStore<UserState, UserId> entityStore) : base(entityStore)
    {
        _eventStore = eventStore;
    }

    public override async Task<User?> GetByIdAsync(UserId aggregateId, CancellationToken ct)
    {
        var userState = await _stateStore.GetAsync(aggregateId, ct);
        if (userState is not null)
            return new User(ref userState);

        var events = await _eventStore.LoadAsync(aggregateId, ct);
        if (events.Count == 0)
            return null;

        var user = new User(events);
        await _stateStore.AddAsync(user.State, ct);

        return user;
    }

    protected override Task SaveEventsAsync(User aggregate, CancellationToken ct)
        => _eventStore.AppendAsync(
                aggregate.Id,
                aggregate.DomainEvents,
                expectedVersion: aggregate.StoredVersion,
                ct);
}

