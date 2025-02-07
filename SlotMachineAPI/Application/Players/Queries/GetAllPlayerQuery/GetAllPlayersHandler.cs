using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Queries.GetAllPlayer
{
    public class GetAllPlayersHandler : IRequestHandler<GetAllPlayersQuery, List<Player>>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<GetAllPlayersHandler> _logger;

        public GetAllPlayersHandler(IPlayerRepository playerRepository, ILogger<GetAllPlayersHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<List<Player>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all Players from the database");
                return await _playerRepository.GetAllAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all Players.");
                throw;
            }
        }
    }

}
