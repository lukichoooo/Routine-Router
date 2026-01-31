using Application.Interfaces.Command;
using OpenAI.Chat;

namespace Infrastructure.Services.Command;


public class CommandParser : ICommandParser
{
    private readonly ChatClient _client;

    public CommandParser(ChatClient client)
    {
        _client = client;
    }

    public Task<object> ParseAsync(string command)
    {
        throw new NotImplementedException();
    }
}

