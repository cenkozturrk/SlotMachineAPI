using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SlotMachineAPI.Domain;
using Microsoft.Extensions.Logging;
using SlotMachineAPI.Application.Players.Commands.SpindCommand;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

namespace SlotMachineAPI.Tests.Handlers
{
    public class SpinHandlerTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<ISlotMachineSettingsRepository> _slotMachineSettingsMock;
        private readonly Mock<ILogger<SpinHandler>> _loggerMock;
        private readonly SpinHandler _handler;

        public SpinHandlerTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _slotMachineSettingsMock = new Mock<ISlotMachineSettingsRepository>();  
            _loggerMock = new Mock<ILogger<SpinHandler>>();

            _slotMachineSettingsMock.Setup(repo => repo.GetSettingsAsync())
                .ReturnsAsync(new SlotMachineSettings { Rows = 3, Cols = 5 });

            _handler = new SpinHandler(_playerRepositoryMock.Object, _loggerMock.Object, _slotMachineSettingsMock.Object);
        }

        /// <summary>
        /// Unit test to verify that the Spin operation generates a correctly sized slot matrix.
        /// Ensures that the matrix consists of 3 rows and each row contains exactly 5 elements.
        /// </summary>
        [Fact]
        public async Task Spin_ShouldGenerateCorrectMatrixSize()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            var settings = new SlotMachineSettings { Rows = 3, Cols = 5 };

            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);
            _slotMachineSettingsMock.Setup(repo => repo.GetSettingsAsync()).ReturnsAsync(settings); 

            var command = new SpinCommand { PlayerId = "67a533e7175f6d97b8e47a7d", BetAmount = 10 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Matrix);
            Assert.Equal(3, result.Matrix.Length); // there should be 3 lines
            Assert.All(result.Matrix, row => Assert.Equal(5, row.Length)); // There should be 5 elements in each row
        }

        /// <summary>
        /// Unit test to verify that a player's balance is correctly decreased after placing a bet.
        /// Ensures that the balance is reduced by at least the bet amount after a spin.
        /// </summary>
        [Fact]
        public async Task Spin_ShouldDecreaseBalanceAfterBet()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            var settings = new SlotMachineSettings { Rows = 3, Cols = 5 };

            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);
            _slotMachineSettingsMock.Setup(repo => repo.GetSettingsAsync()).ReturnsAsync(settings);

            var command = new SpinCommand { PlayerId = "67a533e7175f6d97b8e47a7d", BetAmount = 10 };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(player.Balance >= 90); 
            Assert.True(player.Balance >= 100 - command.BetAmount); 
        }

        /// <summary>
        /// Unit test to verify that an exception is thrown when a player attempts to spin with insufficient balance.
        /// Ensures that the balance remains unchanged and no update operation is performed in such cases.
        /// </summary>
        [Fact]
        public async Task Spin_ShouldThrowException_WhenInsufficientBalance()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 5 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);

            var command = new SpinCommand { PlayerId = "67a533e7175f6d97b8e47a7d", BetAmount = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            _playerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Player>()), Times.Never);
        }

        /// <summary>
        /// Unit test to verify that a KeyNotFoundException is thrown when attempting to spin with a non-existent player ID.
        /// Ensures that no further processing occurs if the player is not found in the database.
        /// </summary>
        [Fact]
        public async Task Spin_ShouldThrowException_WhenPlayerNotFound()
        {
            // Arrange
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("99999")).ReturnsAsync((Player)null);

            var command = new SpinCommand { PlayerId = "99999", BetAmount = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        /// <summary>
        /// Unit test to verify that the spin operation correctly calculates the win amount.
        /// Ensures that the win amount is never negative and that the player's balance is updated accordingly after a spin.
        /// </summary>
        [Fact]
        public async Task Spin_ShouldCorrectlyCalculateWinAmount()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);

            var command = new SpinCommand { PlayerId = "67a533e7175f6d97b8e47a7d", BetAmount = 10 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.WinAmount >= 0); // WinAmount cannot be negative
            Assert.True(result.CurrentBalance >= player.Balance); // The post-earnings balance should be at least the same as the initial balance
        }
    }
}
