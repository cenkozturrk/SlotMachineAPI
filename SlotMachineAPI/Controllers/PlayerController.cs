using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlotMachineAPI.Application.Players.Commands;
using SlotMachineAPI.Application.Players.Commands.CreatePlayerCommand;
using SlotMachineAPI.Application.Players.Commands.DeletePlayerCommand;
using SlotMachineAPI.Application.Players.Commands.UpdatePlayerCommand;
using SlotMachineAPI.Application.Players.Queries;
using SlotMachineAPI.Application.Players.Queries.GetAllPlayer;
using SlotMachineAPI.Application.Players.Queries.GetPlayerQuery;

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _mediator.Send(new GetAllPlayersQuery());
            if (players is null || !players.Any())
                throw new KeyNotFoundException("The player list could not be found!"); 

            return Ok(players);
        }

        [Authorize(Roles = "Admin,Employer")]
        [HttpGet("id")]
        public async Task<IActionResult> GetPlayer([FromQuery] GetPlayerQuery query)
        {
            var player = await _mediator.Send(query);
            if (player is null)
                throw new KeyNotFoundException($"Player with ID {query.Id} not found!");

            return Ok(player);
        }

        [Authorize(Roles = "Admin,Employer")]
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

        [Authorize(Roles = "Admin,Employer")] 
        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance([FromBody] UpdateBalanceCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success)
                throw new KeyNotFoundException("Insufficient balance or player not found");

            return Ok(new
            {
                message = "Balance updated successfully"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> DeletePlayer([FromBody] DeletePlayerCommand command)
        {
            var success = await _mediator.Send(command);
            if (!success)
                throw new KeyNotFoundException("The player could not be deleted!");

            return Ok(new
            {
                message = "Player deleted successfully"
            });
        }

    }
}
