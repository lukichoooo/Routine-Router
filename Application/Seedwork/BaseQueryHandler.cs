using MediatR;

namespace Application.Seedwork;


/// <summary>
/// Makes No Changes To Database
/// </summary>
public abstract class BaseQueryHandler<TQuery, TRes> : IRequestHandler<TQuery, TRes>
where TQuery : IQuery<TRes>
{
    public async Task<TRes> Handle(
            TQuery query,
            CancellationToken ct)
    {
        return await Execute(query, ct);
    }

    protected abstract Task<TRes> Execute(
            TQuery query,
            CancellationToken ct);
}

