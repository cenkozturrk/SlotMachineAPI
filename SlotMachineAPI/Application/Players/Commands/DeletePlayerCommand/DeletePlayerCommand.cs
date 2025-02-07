using MediatR;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.DeletePlayerCommand
{
    public class DeletePlayerCommand : IRequest<bool>
    {
        public string PlayerId { get; set; }
    }    
}
