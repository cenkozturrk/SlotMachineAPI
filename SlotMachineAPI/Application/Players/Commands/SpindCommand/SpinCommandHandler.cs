using MediatR;
using Serilog;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

public class SpinHandler : IRequestHandler<SpinCommand, SpinResult>
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ILogger<SpinHandler> _logger;
    private readonly ISlotMachineSettingsRepository _settingsRepository;
    private readonly Random _random = new();

    public SpinHandler(IPlayerRepository playerRepository, ILogger<SpinHandler> logger, ISlotMachineSettingsRepository settingsRepository)
    {
        _playerRepository = playerRepository;
        _logger = logger;
        _settingsRepository = settingsRepository;
    }

    /// <summary>
    /// Handles the slot machine spin operation.
    /// This method retrieves the player from the database, checks their balance,
    /// generates a slot machine matrix, calculates winnings, and updates the player's balance accordingly.
    /// </summary>
    /// <param name="request">The spin request containing the Player ID and Bet Amount.</param>
    /// <param name="cancellationToken">Token for task cancellation.</param>
    /// <returns>
    /// Returns a <see cref="SpinResult"/> object containing the generated slot matrix, win amount, 
    /// and the player's updated balance.
    /// </returns>
    /// <exception cref="KeyNotFoundException">Thrown if the player is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the player has insufficient balance.</exception>
    public async Task<SpinResult> Handle(SpinCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing spin for PlayerId: {PlayerId} with BetAmount: {BetAmount}", request.PlayerId, request.BetAmount);

        var player = await _playerRepository.GetByIdAsync(request.PlayerId);
        if (player is null)
        {
            _logger.LogWarning("Player with ID {PlayerId} not found!", request.PlayerId);
            throw new KeyNotFoundException($"Player with ID {request.PlayerId} not found!");
        }

        if (player.Balance < request.BetAmount)
        {
            _logger.LogWarning("Insufficient balance for PlayerId: {PlayerId}", request.PlayerId);
            throw new InvalidOperationException("Insufficient balance.");
        }

        // Reduce betAmount from the player's balance
        player.Balance -= request.BetAmount;

        // Get the slot machine size from MongoDB
        var settings = await _settingsRepository.GetSettingsAsync();
        if (settings == null)
        {
            _logger.LogError("Slot machine settings not found in the database!");
            throw new InvalidOperationException("Slot machine settings not configured!");
        }

        int rows = settings.Rows;
        int cols = settings.Cols;

        int[][] matrix = GenerateSlotMatrix(rows, cols);

        // Calculation of earnings
        decimal winAmount = CalculateTotalWin(matrix, request.BetAmount);

        // Add earning
        player.Balance += winAmount;

        // Save updated player data to MongoDB
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
    /// Creates a random matrix for the slot machine.
    /// </summary>
    private int[][] GenerateSlotMatrix(int rows, int cols)
    {
        int[][] matrix = new int[rows][];
        for (int i = 0; i < rows; i++)
        {
            matrix[i] = new int[cols];
            for (int j = 0; j < cols; j++)
            {
                matrix[i][j] = _random.Next(0, 10); // Random numbers from 0-9
            }
        }
        return matrix;
    }

    /// <summary>
    /// It calculates the total winnings by checking all the winning combinations.
    /// </summary>
    private decimal CalculateTotalWin(int[][] matrix, decimal betAmount)
    {
        decimal totalWin = 0;

        // Check horizontal lines
        foreach (var row in matrix)
        {
            totalWin += CalculateLineWin(row, betAmount);
        }

        // Check the cross lines
        totalWin += CalculateDiagonalWins(matrix, betAmount);

        return totalWin;
    }

    /// <summary>
    /// Performs gain calculation for horizontal and diagonal lines.
    /// </summary>
    private decimal CalculateLineWin(int[] line, decimal betAmount)
    {
        decimal win = 0;
        int count = 1;

        for (int i = 1; i < line.Length; i++)
        {
            if (line[i] == line[i - 1])
            {
                count++;
            }
            else
            {
                if (count >= 3) // There must be at least 3 identical symbols
                {
                    win += betAmount * line[i - 1] * count; // Win = bet * number * consecutive length
                }
                count = 1;
            }
        }

        if (count >= 3)
        {
            win += betAmount * line[line.Length - 1] * count;
        }

        return win;
    }

    /// <summary>
    /// Makes a gain calculation for cross lines.
    /// </summary>
    private decimal CalculateDiagonalWins(int[][] matrix, decimal betAmount)
    {
        decimal diagonalWin = 0;
        int rows = matrix.Length;
        int cols = matrix[0].Length;

        // Diagonal from top left to bottom right
        int[] diagonal1 = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            diagonal1[i] = matrix[i][i];
        }
        diagonalWin += CalculateLineWin(diagonal1, betAmount);

        // Diagonal from top right to bottom left
        int[] diagonal2 = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            diagonal2[i] = matrix[i][cols - 1 - i];
        }
        diagonalWin += CalculateLineWin(diagonal2, betAmount);

        return diagonalWin;
    }
}

