using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

namespace SlotMachineAPI.Application.Players.Queries.GetPlayerQuery
{
    public class GetPlayerHandler : IRequestHandler<GetPlayerQuery, Player>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<GetPlayerHandler> _logger;

        public GetPlayerHandler(IPlayerRepository playerRepository, ILogger<GetPlayerHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of a specific player from the database using their unique ID.
        /// </summary>
        /// <param name="request">The request object containing the player's ID.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>The player object if found; otherwise, an exception is thrown.</returns>
        /// <exception cref="Exception">Logs and rethrows any unexpected errors that occur during data retrieval.</exception>
        public async Task<Player> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching Player by Id({request.Id}) from the MongoDb.");

                return await _playerRepository.GetByIdAsync(request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the player with ID {request.Id}.");
                throw;
            }

        }
    }

}
