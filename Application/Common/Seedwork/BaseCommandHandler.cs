using Application.Interfaces.Data;
using MediatR;

namespace Application.Common.Seedwork;

/// <summary>
/// Saves Changes to Database Automatically
/// (Returns result)
/// </summary>
public abstract class BaseCommandHandler<TResult> : IRequestHandler<ICommand<TResult>, TResult>
{
    private readonly IUnitOfWork _uow;

    protected BaseCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<TResult> Handle(
            ICommand<TResult> command,
            CancellationToken ct)
    {
        var result = await ExecuteAsync(command, ct);
        await _uow.CommitAsync(ct);
        return result;
    }

    protected abstract Task<TResult> ExecuteAsync(
            ICommand<TResult> command,
            CancellationToken ct);
}


/// <summary>
/// Saves Changes to Database Automatically
/// (Doesn't return anything)
/// </summary>
public abstract class BaseCommandHandler : IRequestHandler<ICommand>
{
    private readonly IUnitOfWork _uow;

    protected BaseCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(
            ICommand command,
            CancellationToken ct)
    {
        await ExecuteAsync(command, ct);
        await _uow.CommitAsync(ct);
    }

    protected abstract Task ExecuteAsync(
            ICommand command,
            CancellationToken ct);
}



