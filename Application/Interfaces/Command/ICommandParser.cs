using Application.Common.Seedwork;

namespace Application.Interfaces.Command;


public interface ICommandParser
{
    Task<ICommand> ParseAsync(string command);
}

