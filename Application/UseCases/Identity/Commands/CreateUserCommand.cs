using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Domain.Common.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;

namespace Application.UseCases.Identity.Commands;


public sealed record CreateUserCommand : ICommand<EmptyReturn>;


public class CreateUserCommandHandler(
        IUserRepo userRepo,
        IIdentityProvider identity,
        IUnitOfWork uow) : BaseCommandHandler<CreateUserCommand, EmptyReturn>(uow)
{
    protected override async Task<EmptyReturn> Execute(CreateUserCommand command, CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        var user = await userRepo.GetById(userId, ct);
        if (user is not null)
            throw new ApplicationArgumentException($"User with Id={userId} already exists");

        user = new User();
        user.Create(
                userId,
                new Name(identity.GetCurrentUserName()),
                new PasswordHash("empty"));

        await userRepo.Add(user, ct);

        return new EmptyReturn();
    }
}

