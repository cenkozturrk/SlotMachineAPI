using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.CreatePlayerCommand
{
    public class CreatePlayerHandler : IRequestHandler<CreatePlayerCommand, string>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<CreatePlayerHandler> _logger;
        public CreatePlayerHandler(IPlayerRepository playerRepository, ILogger<CreatePlayerHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the creation of a new player.
        /// Ensures the request is valid, assigns a default balance, and saves the player to the database.
        /// </summary>
        /// <param name="request">The player creation request containing the player's name.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>The unique identifier of the newly created player.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
        /// <exception cref="Exception">Logs and rethrows any unexpected errors during the player creation process.</exception>
        public async Task<string> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding a new Player.");
            try
            {
                if (request is null)
                {
                    _logger.LogInformation("A new player could not be created because the user did not enter player information.");
                    throw new ArgumentNullException(nameof(request), "Player cannot be null.");
                }
                _logger.LogInformation("Adding new player with name: {PlayerName}", request.Name);

                var player = new Player
                {
                    Name = request.Name,
                    Balance = 100.00m
                };

                await _playerRepository.AddAsync(player);
                return player.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the player.");
                throw;
            }

        }
    }

}
