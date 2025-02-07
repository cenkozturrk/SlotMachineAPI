using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Queries.GetAllPlayer
{
    public class GetAllPlayersQuery : IRequest<List<Player>>
    {
    }
}
