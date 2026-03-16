using Application.Interfaces.Command;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[ApiController]
[Route("[controller]")]
public class ChatController(IInputParser parser, ISender sender) : ControllerBase
{
    [HttpPost("command")]
    public async Task<ActionResult<string>> Command([FromBody] string input)
    {
        dynamic cmd = await parser.Parse(input);

        dynamic result = await sender.Send(cmd);

        return Ok(result);
    }

    [HttpPost("query")]
    public async Task<ActionResult<string>> Query([FromBody] string input)
    {
        dynamic cmd = await parser.Parse(input);

        dynamic result = await sender.Send(cmd);

        return Ok(result);
    }
}
