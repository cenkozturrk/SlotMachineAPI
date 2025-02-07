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
                    matrix[i][j] = _random.Next(0, 7); 

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

            foreach (var row in matrix)
            {
                int consecutiveSum = row[0];
                int count = 1;

                for (int j = 1; j < row.Length; j++)
                {
                    if (row[j] == row[j - 1])
                    {
                        consecutiveSum += row[j];
                        count++;
                    }
                    else
                    {
                        if (count > 2) // En az 3 aynı sayı olmalı (Task'e uygun)
                        {
                            totalWin += (betAmount * consecutiveSum) * 0.5m; // Çarpanı 0.5 yaptık!
                        }

                        consecutiveSum = row[j];
                        count = 1;
                    }
                }

                if (count > 2)
                {
                    totalWin += (betAmount * consecutiveSum) * 0.5m;
                }
            }

            return totalWin;
        }
    }
}
