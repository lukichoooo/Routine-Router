using Application.Interfaces.Data;
using MediatR;

namespace Application.Common.Seedwork;

/// <summary>
/// Saves Changes to Database Automatically
/// (Returns result)
/// </summary>
public abstract class BaseCommandHandler<TCommand, TResult>
: IRequestHandler<TCommand, TResult>
where TCommand : ICommand<TResult>
{
    private readonly IUnitOfWork _uow;

    protected BaseCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TResult> Handle(
            TCommand command,
            CancellationToken ct)
    {
        var result = await ExecuteAsync(command, ct);
        await _uow.CommitAsync(ct);
        return result;
    }

    protected abstract Task<TResult> ExecuteAsync(
            TCommand command,
            CancellationToken ct);
}

