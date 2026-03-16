using Application.Interfaces.Command;
using Application.UseCases.Identity.Commands;
using Application.UseCases.Schedules.Commands;
using Application.UseCases.Schedules.Queries;
using OpenAI.Chat;

namespace Infrastructure.Services.Command;


public class LLMInputParser(ChatClient client) : IInputParser
{
    public Task<object> Parse(string command)
    {
        // TODO: implement

        return Task.FromResult((object)new GetTodaysChecklistsQuery());
        // return Task.FromResult((object)new CreateChecklistCommand());
        // return Task.FromResult<ICommand<object>>(new CreateUserCommand());
    }
}

