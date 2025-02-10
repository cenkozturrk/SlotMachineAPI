using MediatR;
using Serilog;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Repositories;

namespace SlotMachineAPI.Application.Players.Commands.SpindCommand
{
    public class SpinHandler : IRequestHandler<SpinCommand, SpinResult>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<SpinHandler> _logger;
        private readonly Random _random = new();
        public SpinHandler(IPlayerRepository playerRepository, ILogger<SpinHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        /// <summary>
        ///  Handles the spin operation for a player.
        ///  Generates a random matrix, calculates winnings based on predefined rules, and updates balance.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<SpinResult> Handle(SpinCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing spin for PlayerId: {PlayerId} with BetAmount: {BetAmount}",
                request.PlayerId, request.BetAmount);

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);

            if (player == null)
            {
                _logger.LogWarning("Player with ID {PlayerId} not found!", request.PlayerId);
                throw new KeyNotFoundException($"Player with ID {request.PlayerId} not found!");
            }

            if (player.Balance < request.BetAmount)
            {
                _logger.LogWarning("Insufficient balance for PlayerId: {PlayerId}", request.PlayerId);
                throw new InvalidOperationException("Insufficient balance.");
            }

            // 1 - Reduce the bet from the player's balance
            player.Balance -= request.BetAmount;

            // 2 - Slot Matrix (5x3) should be randomly generated
            int rows = 3, cols = 5;
            int[][] matrix = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = _random.Next(0, 10); // Random numbers from 0-9
                }
            }

            // 3 - Calculate winnings based on predefined slot machine rules
            decimal winAmount = CalculateWin(matrix, request.BetAmount);

            // 4 - Add winnings to the player's balance
            player.Balance += winAmount;

            // 5 - Save updated player data
            await _playerRepository.UpdateAsync(player.Id, player);

            _logger.LogInformation("Spin completed. PlayerId: {PlayerId}, WinAmount: {WinAmount}, NewBalance: {NewBalance}",
                request.PlayerId, winAmount, player.Balance);

            return new SpinResult
            {
                Matrix = matrix,
                WinAmount = winAmount,
                CurrentBalance = player.Balance
            };
        }

        /// <summary>
        /// Calculates the winnings based on the slot machine logic.
        /// </summary>
        /// <param name="matrix">Generated slot matrix</param>
        /// <param name="betAmount">Player's bet amount</param>
        /// <returns></returns>
        private decimal CalculateWin(int[][] matrix, decimal betAmount)
        {
            decimal totalWin = 0;

            // 1 - Horizontal (row) controls: There must be 3 or more numbers in the same order
            foreach (var row in matrix)
            {
                totalWin += CalculateLineWin(row, betAmount);
            }

            // 2. Diagonal control from top left to bottom right
            int[] diagonal1 = { matrix[0][0], matrix[1][1], matrix[2][2] };
            totalWin += CalculateLineWin(diagonal1, betAmount);

            // 3. Diagonal control from top right to bottom left
            int[] diagonal2 = { matrix[0][2], matrix[1][1], matrix[2][0] };
            totalWin += CalculateLineWin(diagonal2, betAmount);

            return totalWin;
        }

        /// <summary>
        /// Calculates the win amount for a given line in the slot machine matrix.
        /// A win occurs when there are three or more consecutive identical numbers.
        /// </summary>
        /// <param name="line">The array representing a single line of the slot machine.</param>
        /// <param name="betAmount">The amount the player has bet.</param>
        /// <returns>The calculated win amount for the given line.</returns>
        private decimal CalculateLineWin(int[] line, decimal betAmount)
        {
            decimal win = 0;
            int consecutiveSum = line[0];
            int count = 1;

            for (int i = 1; i < line.Length; i++)
            {
                if (line[i] == line[i - 1])
                {
                    consecutiveSum += line[i];
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        win += betAmount * consecutiveSum;
                    }
                    consecutiveSum = line[i];
                    count = 1;
                }
            }

            if (count >= 3)
            {
                win += betAmount * consecutiveSum;
            }

            return win;
        }
    }
}
