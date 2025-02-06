using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Queries
{
    public class GetAllPlayersQuery : IRequest<List<Player>>
    {
    }
    public class GetAllPlayersHandler : IRequestHandler<GetAllPlayersQuery, List<Player>>
    {
        private readonly IPlayerRepository _playerRepository;

        public GetAllPlayersHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<List<Player>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
        {
            return await _playerRepository.GetAllAsync();
        }
    }
}
