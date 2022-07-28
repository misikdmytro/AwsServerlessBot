using System.Threading;
using System.Threading.Tasks;

using LambdaBot.Services;

using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

namespace LambdaBot.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BotController : ControllerBase
{
    private readonly IUpdateService _updateService;

    public BotController(IUpdateService updateService)
    {
        _updateService = updateService;
    }

    [HttpPost]
    public async Task<IActionResult> HandleUpdate([FromBody] Update update,
        CancellationToken cancellationToken = default)
    {
        await _updateService.HandleUpdate(update, cancellationToken);
        return Ok();
    }
}