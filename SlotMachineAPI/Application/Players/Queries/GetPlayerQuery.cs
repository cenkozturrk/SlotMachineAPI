using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Queries
{
    public class GetPlayerQuery : IRequest<Player>
    {
        public string Id { get; set; }
    }
    public class GetPlayerHandler : IRequestHandler<GetPlayerQuery, Player>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<GetPlayerHandler> _logger;

        public GetPlayerHandler(IPlayerRepository playerRepository, ILogger<GetPlayerHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

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
