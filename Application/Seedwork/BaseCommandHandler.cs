using Application.Interfaces.Data;
using MediatR;

namespace Application.Seedwork;

/// <summary>
/// Saves Changes to Database Automatically
/// (Returns result)
/// </summary>
public abstract class BaseCommandHandler<TCmd, TRes>
: IRequestHandler<TCmd, TRes>
where TCmd : ICommand<TRes>
{
    private readonly IUnitOfWork _uow;

    protected BaseCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TRes> Handle(
            TCmd command,
            CancellationToken ct)
    {
        var result = await ExecuteAsync(command, ct);
        await _uow.CommitAsync(ct);
        return result;
    }

    protected abstract Task<TRes> ExecuteAsync(
            TCmd command,
            CancellationToken ct);
}

