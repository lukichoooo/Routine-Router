using Application.Interfaces.Command;
using MediatR;

namespace CLI;


public interface IController
{
    public void Handle(string input);
}

public class ConsoleController : IController
{
    private readonly ICommandParser _parser;
    private readonly ISender _sender;

    public ConsoleController(ICommandParser parser, ISender sender)
    {
        _parser = parser;
        _sender = sender;
    }

    public async void Handle(string input)
    {
        dynamic cmd = await _parser.ParseAsync(input);

        await _sender.Send(cmd);

        //
        // if (result != null)
        //     Console.WriteLine(result);
    }
}
