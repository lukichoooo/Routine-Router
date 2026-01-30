using MediatR;

namespace Application.Common.Seedwork;

public interface IQuery<TResult> : IRequest<TResult>;

