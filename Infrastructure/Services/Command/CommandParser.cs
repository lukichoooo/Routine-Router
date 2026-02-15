using Application.Interfaces.Command;
using Application.Seedwork;
using Application.UseCases.Identity.Commands;
using Application.UseCases.Schedules.Commands;
using OpenAI.Chat;

namespace Infrastructure.Services.Command;


public class CommandParser(ChatClient client) : ICommandParser
{
    public Task<ICommand<object>> Parse(string command)
    {
        // TODO: implement

        return Task.FromResult<ICommand<object>>(new CreateChecklistCommand());
        // return Task.FromResult<ICommand<object>>(new CreateUserCommand());
    }
}

