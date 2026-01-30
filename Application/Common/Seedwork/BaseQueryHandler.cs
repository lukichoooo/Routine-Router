using MediatR;

namespace Application.Common.Seedwork;


/// <summary>
/// Makes No Changes To Database
/// </summary>
public abstract class BaseQueryHandler<TResult> : IRequestHandler<IQuery<TResult>, TResult>
{
    public async Task<TResult> Handle(
            IQuery<TResult> query,
            CancellationToken ct)
    {
        return await ExecuteAsync(query, ct);
    }

    protected abstract Task<TResult> ExecuteAsync(
            IQuery<TResult> command,
            CancellationToken ct);
}

