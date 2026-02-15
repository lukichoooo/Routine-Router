using Application.Interfaces.Data;
using MediatR;

namespace Application.Seedwork;

/// <summary>
/// Saves Changes to Database Automatically
/// (Returns result)
/// </summary>
public abstract class BaseCommandHandler<TCmd, TRes>(IUnitOfWork uow)
: IRequestHandler<TCmd, TRes>
where TCmd : ICommand<TRes>
{
    public async Task<TRes> Handle(
            TCmd command,
            CancellationToken ct)
    {
        var result = await Execute(command, ct);
        await uow.Commit(ct);
        return result;
    }

    protected abstract Task<TRes> Execute(
            TCmd command,
            CancellationToken ct);
}

