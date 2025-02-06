using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands
{
    public class CreatePlayerCommand : IRequest<string>
    {
        public string Name { get; set; }
    }
    public class CreatePlayerHandler : IRequestHandler<CreatePlayerCommand, string>
    {
        private readonly IPlayerRepository _playerRepository;

        public CreatePlayerHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        public async Task<string> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var player = new Player
            {
                Name = request.Name,
                Balance = 100.00m
            };

            await _playerRepository.AddAsync(player);
            return player.Id;
        }
    }
}
