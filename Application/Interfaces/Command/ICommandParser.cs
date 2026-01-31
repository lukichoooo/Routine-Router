namespace Application.Interfaces.Command;


public interface ICommandParser
{
    Task<object> ParseAsync(string command);
}

