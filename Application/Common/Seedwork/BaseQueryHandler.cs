using MediatR;

namespace Application.Common.Seedwork;


/// <summary>
/// Makes No Changes To Database
/// </summary>
public abstract class BaseQueryHandler<TRes> : IRequestHandler<IQuery<TRes>, TRes>
{
    public async Task<TRes> Handle(
            IQuery<TRes> query,
            CancellationToken ct)
    {
        return await ExecuteAsync(query, ct);
    }

    protected abstract Task<TRes> ExecuteAsync(
            IQuery<TRes> command,
            CancellationToken ct);
}

