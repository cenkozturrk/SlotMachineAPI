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

        public GetPlayerHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<Player> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
        {
            return await _playerRepository.GetByIdAsync(request.Id);
        }
    }
}
