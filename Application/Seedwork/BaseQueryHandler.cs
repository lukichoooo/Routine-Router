using MediatR;

namespace Application.Seedwork;


/// <summary>
/// Makes No Changes To Database
/// </summary>
public abstract class BaseQueryHandler<TRes> : IRequestHandler<IQuery<TRes>, TRes>
{
    public async Task<TRes> Handle(
            IQuery<TRes> query,
            CancellationToken ct)
    {
        return await Execute(query, ct);
    }

    protected abstract Task<TRes> Execute(
            IQuery<TRes> command,
            CancellationToken ct);
}

