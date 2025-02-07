using MediatR;
using Serilog;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.SpindCommand
{
    public class SpinCommandHandler : IRequestHandler<SpinCommand, SpinResult>
    {

        private readonly IPlayerRepository _playerRepository;
        private readonly Random _random = new Random();

        public SpinCommandHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<SpinResult> Handle(SpinCommand request, CancellationToken cancellationToken)
        {
            Log.Information(" The spin process has been started! PlayerID: {PlayerId}, BetAmount: {BetAmount}",
                request.PlayerId, request.BetAmount);

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);

            if (player == null || player.Balance < request.BetAmount)
            {
                Log.Warning(" The spin operation failed! The player could not be found or the balance is insufficient.PlayerID: {PlayerId}, Balance: {Balance}, BetAmount: {BetAmount}",
                    request.PlayerId, player?.Balance ?? 0, request.BetAmount);

                return new SpinResult
                {
                    Matrix = null,
                    WinAmount = 0,
                    CurrentBalance = player?.Balance ?? 0
                };
            }

            player.Balance -= request.BetAmount;

            int rows = 3, cols = 5;
            int[][] matrix = GenerateRandomMatrix(rows, cols);

            decimal winAmount = CalculateWin(matrix, request.BetAmount);

            player.Balance += winAmount;

            await _playerRepository.UpdateAsync(player.Id, player);

            Log.Information(" The spin is complete! WinAmount: {WinAmount}, NewBalance: {NewBalance}",
                winAmount, player.Balance);

            return new SpinResult
            {
                Matrix = matrix,
                WinAmount = winAmount,
                CurrentBalance = player.Balance
            };
        }

        private int[][] GenerateRandomMatrix(int rows, int cols)
        {
            int[][] matrix = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = _random.Next(0, 10);
                }
            }
            return matrix;
        }

        private decimal CalculateWin(int[][] matrix, decimal betAmount)
        {
            decimal totalWin = 0;

            foreach (var row in matrix)
            {
                totalWin += CalculateLineWin(row, betAmount);
            }

            totalWin += CalculateDiagonalWin(matrix, betAmount);

            return totalWin;
        }

        private decimal CalculateLineWin(int[] line, decimal betAmount)
        {
            decimal win = 0;
            int consecutiveSum = line[0], count = 1;

            for (int i = 1; i < line.Length; i++)
            {
                if (line[i] == line[i - 1])
                {
                    consecutiveSum += line[i];
                    count++;
                }
                else
                {
                    if (count > 2)
                    {
                        win += betAmount * consecutiveSum;
                    }
                    consecutiveSum = line[i];
                    count = 1;
                }
            }

            if (count > 2)
            {
                win += betAmount * consecutiveSum;
            }

            return win;
        }

        private decimal CalculateDiagonalWin(int[][] matrix, decimal betAmount)
        {
            decimal diagonalWin = 0;
            int rows = matrix.Length, cols = matrix[0].Length;

            diagonalWin += CalculateDiagonal(matrix, 0, 0, 1, 1, betAmount);

            diagonalWin += CalculateDiagonal(matrix, 0, 0, 1, 1, betAmount);
            diagonalWin += CalculateDiagonal(matrix, rows - 1, 0, -1, 1, betAmount);

            return diagonalWin;
        }

        private decimal CalculateDiagonal(int[][] matrix, int startX, int startY, int stepX, int stepY, decimal betAmount)
        {
            decimal win = 0;
            int x = startX, y = startY;
            int consecutiveSum = matrix[x][y], count = 1;

            while (x + stepX >= 0 && x + stepX < matrix.Length && y + stepY < matrix[0].Length)
            {
                x += stepX;
                y += stepY;

                if (matrix[x][y] == matrix[x - stepX][y - stepY])
                {
                    consecutiveSum += matrix[x][y];
                    count++;
                }
                else
                {
                    if (count > 2)
                    {
                        win += betAmount * consecutiveSum;
                    }
                    consecutiveSum = matrix[x][y];
                    count = 1;
                }
            }

            if (count > 2)
            {
                win += betAmount * consecutiveSum;
            }

            return win;
        }
    }

}
