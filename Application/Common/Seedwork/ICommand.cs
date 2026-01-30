using MediatR;

namespace Application.Common.Seedwork;

public interface ICommand<T> : IRequest<T>;
public interface ICommand : IRequest;

