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

            if (players is null || !players.Any())
                throw new KeyNotFoundException("The player list could not be found!"); 

            return Ok(players);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayer(string id)
        {
            var player = await _mediator.Send(new GetPlayerQuery { Id = id });

            if (player is null)
                throw new KeyNotFoundException($"Player with ID {id} not found!");

            return Ok(player);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerCommand command)
        {
            var playerId = await _mediator.Send(command);
            if (playerId is null)
                throw new KeyNotFoundException("Player could not be created!");

            return Ok(new
            {
                message = "Player created",
                playerId
            });
        }

        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance(string playerId, decimal amount)
        {
            var success = await _mediator.Send( new UpdateBalanceCommand { PlayerId = playerId ,Amount = amount});

            if (!success)
                throw new KeyNotFoundException("Insufficient balance or player not found");

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
                throw new KeyNotFoundException("The player could not be deleted!");

            return Ok(new
            {
                message = "Player deleted successfully"
            });
        }

        [HttpPost("spin/{playerId}")]
        public async Task<IActionResult> Spin(string playerId, decimal betAmount)
        {
            var player = await _mediator.Send(new SpinCommand { PlayerId = playerId , BetAmount = betAmount });

            if (player is null)
                return NotFound(new
                {
                    message = "Player not found"
                });

            return Ok(player);
        }
    }
}
