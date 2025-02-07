using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands
{
    public class UpdateBalanceCommand : IRequest<bool>
    {
        public string PlayerId
        {
            get; set;
        }
        public decimal Amount
        {
            get; set;
        }
    }

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
            _logger.LogInformation("Updating existing Player");

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (player is not null && player.Balance + request.Amount > 0)
            {
                _logger.LogInformation("Updating player with Id {PlayerId}", request.PlayerId);
                player.Balance += request.Amount;
                await _playerRepository.UpdateAsync(request.PlayerId, player);
                return true;
            }
            else
            {
                _logger.LogWarning("Player with ID {PlayerId} not found.", request.PlayerId);
                throw new KeyNotFoundException($"Player with ID {request.PlayerId} not found.");
            }
        }
    }
}
