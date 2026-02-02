using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos.Common;

namespace Infrastructure.Repos;


public class UserRepo : BaseRepository<User, UserId>, IUserRepo
{
    private readonly IEventStore _eventStore;

    public UserRepo(
            IEventStore eventStore,
            ITrackedEntities trackedEntities) : base(trackedEntities)
    {
        _eventStore = eventStore;
    }

    public override Task<User?> GetByIdAsync(UserId aggregateId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    protected override Task SaveAsyncProtected(User aggregate, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

