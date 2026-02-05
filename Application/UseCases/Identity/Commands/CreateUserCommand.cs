using Application.Common.Exceptions;
using Application.Interfaces.Data;
using Application.Seedwork;
using Domain.Entities.Users;

namespace Application.UseCases.Identity.Commands;


public sealed record CreateUserCommand : ICommand<EmptyReturn>;


public class CreateUserCommandHandler : BaseCommandHandler<CreateUserCommand, EmptyReturn>
{
    private readonly IIdentityProvider _identity;
    private readonly IUserRepo _userRepo;

    public CreateUserCommandHandler(
            IUserRepo userRepo,
            IIdentityProvider identity,
            IUnitOfWork uow) : base(uow)
    {
        _identity = identity;
        _userRepo = userRepo;
    }

    protected override async Task<EmptyReturn> ExecuteAsync(CreateUserCommand command, CancellationToken ct)
    {
        var userId = _identity.GetCurrentUserId();
        var user = await _userRepo.GetByIdAsync(userId, ct);
        if (user is not null)
            throw new ApplicationArgumentException($"User with Id={userId} already exists");

        user = new User();
        user.Create(
                userId,
                new(_identity.GetCurrentUserName()),
                new("empty"));

        await _userRepo.AddAsync(user, ct);

        return new EmptyReturn();
    }
}

