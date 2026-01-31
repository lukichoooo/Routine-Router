using MediatR;

namespace Application.Common.Seedwork;

public interface ICommand<out T> : ICommand, IRequest<T>;
public interface ICommand : IRequest;

