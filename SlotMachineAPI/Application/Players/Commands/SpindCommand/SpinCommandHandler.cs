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

            // 1. Oyuncunun bakiyesinden bahsi düş
            player.Balance -= request.BetAmount;

            // 2. Slot Matrix (5x3) rastgele oluşturulmalı
            int rows = 3, cols = 5;
            int[][] matrix = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = _random.Next(0, 10); // 0-9 arası rastgele sayılar
                }
            }

            // 3. Kazanç hesaplanmalı
            decimal winAmount = CalculateWin(matrix, request.BetAmount);

            // 4. Kazanç oyuncunun bakiyesine eklenmeli
            player.Balance += winAmount;

            // 5. Güncellenmiş oyuncu verisini kaydet
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

        private decimal CalculateWin(int[][] matrix, decimal betAmount)
        {
            decimal totalWin = 0;

            // 1. Yatay (row) kontrolleri: Aynı sırada 3 veya daha fazla sayı olmalı
            foreach (var row in matrix)
            {
                totalWin += CalculateLineWin(row, betAmount);
            }

            // 2. Sol üstten sağ alta diagonal kontrolü
            int[] diagonal1 = { matrix[0][0], matrix[1][1], matrix[2][2] };
            totalWin += CalculateLineWin(diagonal1, betAmount);

            // 3. Sağ üstten sol alta diagonal kontrolü
            int[] diagonal2 = { matrix[0][2], matrix[1][1], matrix[2][0] };
            totalWin += CalculateLineWin(diagonal2, betAmount);

            return totalWin;
        }

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
