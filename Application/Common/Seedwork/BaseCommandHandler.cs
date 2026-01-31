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

/// <summary>
/// Saves Changes to Database Automatically
/// (Doesn't return anything)
/// </summary>
public abstract class BaseCommandHandler<TCommand>
: IRequestHandler<TCommand>
where TCommand : ICommand
{
    private readonly IUnitOfWork _uow;

    protected BaseCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task Handle(
            TCommand command,
            CancellationToken ct)
    {
        await ExecuteAsync(command, ct);
        await _uow.CommitAsync(ct);
    }

    protected abstract Task ExecuteAsync(
            TCommand command,
            CancellationToken ct);
}



