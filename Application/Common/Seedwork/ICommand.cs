using MediatR;

namespace Application.Common.Seedwork;

public interface ICommand<out TResult> : IRequest<TResult>;

