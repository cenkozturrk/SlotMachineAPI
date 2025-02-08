using MediatR;
using SlotMachineAPI.Infrastructure.Repositories;

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
        public async Task<bool> Handle(UpdateBalanceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating balance for Player with ID: {PlayerId}", request.PlayerId);

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);

            // Eğer oyuncu bulunamazsa
            if (player is null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found", request.PlayerId);
                throw new KeyNotFoundException($"Player with ID {request.PlayerId} not found.");
            }

            // Eğer bakiye yetersizse
            if (player.Balance + request.Amount < 0)
            {
                _logger.LogWarning("Player with ID {PlayerId} has insufficient balance. Current Balance: {Balance}, Attempted Change: {Amount}",
                    request.PlayerId, player.Balance, request.Amount);

                throw new InvalidOperationException("Balance cannot be negative.");
            }

            // Güncelleme işlemi
            player.Balance += request.Amount;
            _logger.LogInformation("Player with ID {PlayerId} balance updated successfully. New Balance: {NewBalance}",
                request.PlayerId, player.Balance);

            await _playerRepository.UpdateAsync(request.PlayerId, player);
            return true;
        }

    }
}
