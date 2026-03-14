using Application.Interfaces.Command;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


public interface IController
{
    public Task<ActionResult<string>> Handle(string input);
}

[ApiController]
[Route("[controller]")]
public class ChatController(ICommandParser parser, ISender sender) : ControllerBase, IController
{
    [HttpPost]
    public async Task<ActionResult<string>> Handle([FromBody] string input)
    {
        dynamic cmd = await parser.Parse(input);

        dynamic result = await sender.Send(cmd);

        return Ok(result);
    }
}
