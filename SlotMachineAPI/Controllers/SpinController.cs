using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlotMachineAPI.Application.Players.Commands;

namespace SlotMachineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpinController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SpinController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("spin/{playerId}")]
        public async Task<IActionResult> Spin(string playerId, decimal betAmount)
        {
            var player = await _mediator.Send(new SpinCommand { PlayerId = playerId, BetAmount = betAmount });

            if (player is null)
                return NotFound(new
                {
                    message = "Player not found"
                });

            return Ok(player);
        }
    }
}
