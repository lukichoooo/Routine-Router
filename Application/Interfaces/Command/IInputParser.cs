namespace Application.Interfaces.Command;


public interface IInputParser
{
    Task<object> Parse(string command);
}

