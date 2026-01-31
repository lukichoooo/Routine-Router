using Application.Common.Seedwork;

namespace Application.Interfaces.Command;


public interface ICommandParser
{
    Task<ICommand<object>> ParseAsync(string command);
}

