using Application.Common.Seedwork;
using Application.Interfaces.Command;
using Application.UseCases.Schedules.Commands.CreateChecklist;
using OpenAI.Chat;

namespace Infrastructure.Services.Command;


public class CommandParser : ICommandParser
{
    private readonly ChatClient _client;

    public CommandParser(ChatClient client)
    {
        _client = client;
    }

    public Task<ICommand> ParseAsync(string command)
    {
        // TODO: implement
        return Task.FromResult<ICommand>(new CreateChecklistCommand());
    }
}

