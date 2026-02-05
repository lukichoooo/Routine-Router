using Application.Interfaces.Command;
using Application.Seedwork;
using Application.UseCases.Identity.Commands;
using Application.UseCases.Schedules.Commands;
using OpenAI.Chat;

namespace Infrastructure.Services.Command;


public class CommandParser : ICommandParser
{
    private readonly ChatClient _client;

    public CommandParser(ChatClient client)
    {
        _client = client;
    }

    public Task<ICommand<object>> ParseAsync(string command)
    {
        // TODO: implement

        return Task.FromResult<ICommand<object>>(new CreateChecklistCommand());
        // return Task.FromResult<ICommand<object>>(new CreateUserCommand());
    }
}

