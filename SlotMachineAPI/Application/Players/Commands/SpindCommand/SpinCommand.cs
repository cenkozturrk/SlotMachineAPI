using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Serilog;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Repositories;

public class SpinCommand : IRequest<SpinResult>
{
    public string PlayerId { get; set; }
    public decimal BetAmount { get; set; }
}
