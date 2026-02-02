using MediatR;

namespace Application.Seedwork;

public interface ICommand<out TResult> : IRequest<TResult>;

