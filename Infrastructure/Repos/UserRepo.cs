using Application.Interfaces.Events;
using Application.UseCases.Identity;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Infrastructure.Persistence;
using Infrastructure.Repos.Common;

namespace Infrastructure.Repos;


public class UserRepo
(IEntityStateStore<UserState, UserId> stateStore, IEventStore eventStore)
    : BaseRepository<User, UserId, UserState>(stateStore, eventStore), IUserRepo
{
}

