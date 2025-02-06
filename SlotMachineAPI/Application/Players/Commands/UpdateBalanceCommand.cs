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

        public UpdateBalanceHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<bool> Handle(UpdateBalanceCommand request, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (player == null || player.Balance + request.Amount < 0)
                return false;

            player.Balance += request.Amount;
            await _playerRepository.UpdateAsync(request.PlayerId, player);
            return true;
        }
    }
}
