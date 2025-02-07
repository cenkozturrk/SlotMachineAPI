using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.UpdatePlayerCommand
{
    public class UpdateBalanceCommand : IRequest<bool>
    {
        public string PlayerId { get; set; }
        public decimal Amount { get; set; }
    }
}
