using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Domain.Entities.Users;

namespace Application.UseCases.Identity.Commands;


public sealed record CreateUserCommand : ICommand<EmptyReturn>;


public class CreateUserCommandHandler(
        IUserRepo userRepo,
        IIdentityProvider identity,
        IUnitOfWork uow) : BaseCommandHandler<CreateUserCommand, EmptyReturn>(uow)
{
    protected override async Task<EmptyReturn> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        var userId = identity.GetCurrentUserId();
        var user = await userRepo.GetById(userId, ct);
        if (user is not null)
            throw new ApplicationArgumentException($"User with Id={userId} already exists");

        user = new User();
        user.Create(
                userId,
                new(identity.GetCurrentUserName()),
                new("empty"));

        await userRepo.AddAsync(user, ct);

        return new EmptyReturn();
    }
}

