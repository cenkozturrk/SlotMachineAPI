using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlotMachineAPI.Application.Players.Commands;
using SlotMachineAPI.Application.Players.Queries;

namespace SlotMachineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _mediator.Send(new GetAllPlayersQuery());
            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayer(string id)
        {
            var player = await _mediator.Send(new GetPlayerQuery { Id = id });
            if (player == null)
                return NotFound(new
                {
                    message = "Player not found"
                });

            return Ok(player);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerCommand command)
        {
            var playerId = await _mediator.Send(command);
            return Ok(new
            {
                message = "Player created",
                playerId
            });
        }

        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance([FromBody] UpdateBalanceCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success)
                return BadRequest(new
                {
                    message = "Insufficient balance or player not found"
                });

            return Ok(new
            {
                message = "Balance updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(string id)
        {
            var success = await _mediator.Send(new DeletePlayerCommand { PlayerId = id });
            if (!success)
                return NotFound(new
                {
                    message = "Player not found"
                });

            return Ok(new
            {
                message = "Player deleted successfully"
            });
        }

        [HttpPost("spin/{playerId}")]
        public async Task<IActionResult> Spin(string playerId, decimal betAmount)
        {
            var player = await _mediator.Send(new SpinCommand { PlayerId = playerId , BetAmount = betAmount });
            if (player == null)
                return NotFound(new
                {
                    message = "Player not found"
                });
            return Ok(player);
        }
    }
}
