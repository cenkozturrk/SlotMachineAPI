using MediatR;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands
{
    public class SpinCommand : IRequest<SpinResult>
    {
        public string PlayerId { get; set; }
        public decimal BetAmount { get; set; }
    }
    public class SpinHandler : IRequestHandler<SpinCommand, SpinResult>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly Random _random = new Random();
        public SpinHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }
        public async Task<SpinResult> Handle(SpinCommand request, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (player == null || player.Balance < request.BetAmount)
            {
                return new SpinResult { Matrix = null, WinAmount = 0, CurrentBalance = player?.Balance ?? 0 };
            }

            player.Balance -= request.BetAmount;

            int rows = 3, cols = 5;
            int[][] matrix = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = _random.Next(0, 10);
                }
            }

            decimal winAmount = CalculateWin(matrix, request.BetAmount);
            player.Balance += winAmount; 

            await _playerRepository.UpdateAsync(player.Id, player);

            return new SpinResult
            {
                Matrix = matrix,
                WinAmount = winAmount,
                CurrentBalance = player.Balance
            };
        }
        private decimal CalculateWin(int[][] matrix, decimal betAmount)
        {
            decimal totalWin = 0;
            for (int i = 0; i < matrix.Length; i++)
            {
                int[] row = matrix[i];
                if (row.Distinct().Count() == 1) 
                {
                    totalWin += betAmount * 2; 
                }
            }
            return totalWin;
        }
    }
}
