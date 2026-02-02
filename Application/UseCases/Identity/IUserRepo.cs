using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Domain.SeedWork;

namespace Application.UseCases.Identity;


public interface IUserRepo : IRepository<User, UserId>
{
}

