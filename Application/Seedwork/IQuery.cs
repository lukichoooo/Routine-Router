using MediatR;

namespace Application.Seedwork;

public interface IQuery<TResult> : IRequest<TResult>;

