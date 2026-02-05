using MediatR;

namespace Application.Seedwork;

public class EmptyReturn;

public interface ICommand<out TResult> : IRequest<TResult>;

