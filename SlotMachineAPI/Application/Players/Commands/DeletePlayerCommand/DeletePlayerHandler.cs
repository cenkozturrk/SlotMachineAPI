using MediatR;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.DeletePlayerCommand
{
    public class DeletePlayerHandler : IRequestHandler<DeletePlayerCommand, bool>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<DeletePlayerHandler> _logger;
        public DeletePlayerHandler(IPlayerRepository playerRepository, ILogger<DeletePlayerHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the deletion of a player from the database.
        /// Generates a unique transaction ID for tracking the delete operation.
        /// </summary>
        /// <param name="request">The request object containing the player's ID to be deleted.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>True if the player was successfully deleted; false if the player was not found.</returns>
        /// <exception cref="Exception">Logs and returns false if an unexpected error occurs during deletion.</exception>

        public async Task<bool> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete player with ID: {PlayerId}", request.PlayerId);

            var transactionId = Guid.NewGuid().ToString(); // imagine that we are in real-time scenario

            _logger.LogInformation("Transaction {TransactionId} started: Deleting player with ID: {Player}", transactionId, request.PlayerId);

            try
            {
                var player = await _playerRepository.GetByIdAsync(request.PlayerId);

                if (player is not null)
                {
                    _logger.LogInformation("Player found: Deleting player with ID {PlayerId}", request.PlayerId);

                    await _playerRepository.DeleteAsync(request.PlayerId);

                    _logger.LogInformation("Transaction {TransactionId} succeeded: Player with ID {PlayerId} deleted successfully.", transactionId, request.PlayerId);
                    _logger.LogInformation("Player with ID {PlayerId} deleted successfully.", request.PlayerId);
                }
                else
                {
                    _logger.LogWarning("Player with ID {PlayerId} not found. Nothing to delete.", request.PlayerId);
                    return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction {TransactionId} failed: An error occurred while deleting the player with ID {PlayerId}.", transactionId, request.PlayerId);

                return false;
            }
            return true;
        }
    }
}
