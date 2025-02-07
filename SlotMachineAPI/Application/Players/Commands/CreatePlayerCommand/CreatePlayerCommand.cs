using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.CreatePlayerCommand
{
    public class CreatePlayerCommand : IRequest<string>
    {
        public string Name { get; set; }
    }
}
