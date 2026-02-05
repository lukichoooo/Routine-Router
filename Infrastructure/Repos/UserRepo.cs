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

    public override Task<User?> GetByIdAsync(UserId aggregateId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    protected override Task SaveEventsAsync(User aggregate, CancellationToken ct)
        => _eventStore.AppendAsync(
                aggregate.Id,
                aggregate.DomainEvents,
                expectedVersion: aggregate.StoredVersion,
                ct);
}

