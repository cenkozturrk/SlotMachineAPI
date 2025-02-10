using MediatR;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

namespace SlotMachineAPI.Application.Players.Commands.UpdatePlayerCommand
{
    public class UpdateBalanceHandler : IRequestHandler<UpdateBalanceCommand, bool>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<UpdateBalanceHandler> _logger;
        public UpdateBalanceHandler(IPlayerRepository playerRepository, ILogger<UpdateBalanceHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the balance update operation for a player.
        /// Ensures that the player exists and has sufficient balance before updating.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> Handle(UpdateBalanceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating balance for Player with ID: {PlayerId}", request.PlayerId);

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);

            if (player is null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found", request.PlayerId);
                throw new KeyNotFoundException($"Player with ID {request.PlayerId} not found.");
            }

            if (player.Balance + request.Amount < 0)
            {
                _logger.LogWarning("Player with ID {PlayerId} has insufficient balance. Current Balance: {Balance}, Attempted Change: {Amount}",
                    request.PlayerId, player.Balance, request.Amount);

                throw new InvalidOperationException("Balance cannot be negative.");
            }

            player.Balance += request.Amount;
            _logger.LogInformation("Player with ID {PlayerId} balance updated successfully. New Balance: {NewBalance}",
                request.PlayerId, player.Balance);

            await _playerRepository.UpdateAsync(request.PlayerId, player);
            return true;
        }

    }
}
