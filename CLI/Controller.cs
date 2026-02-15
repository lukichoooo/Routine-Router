using Application.Interfaces.Command;
using MediatR;

namespace CLI;


public interface IController
{
    public void Handle(string input);
}

public class ConsoleController(ICommandParser parser, ISender sender) : IController
{
    public async void Handle(string input)
    {
        dynamic cmd = await parser.Parse(input);

        var result = await sender.Send(cmd);

        if (result != null)
            Console.WriteLine(result);
    }
}
