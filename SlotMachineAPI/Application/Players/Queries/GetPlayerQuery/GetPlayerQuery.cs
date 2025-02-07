using MediatR;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Queries.GetPlayerQuery
{
    public class GetPlayerQuery : IRequest<Player>
    {
        public string Id { get; set; }
    }
}
