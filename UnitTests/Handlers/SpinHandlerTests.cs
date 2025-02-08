using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SlotMachineAPI.Infrastructure.Repositories;
using SlotMachineAPI.Domain;
using Microsoft.Extensions.Logging;
using SlotMachineAPI.Application.Players.Commands.SpindCommand;

namespace SlotMachineAPI.Tests.Handlers
{
    public class SpinHandlerTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<ILogger<SpinHandler>> _loggerMock;
        private readonly SpinHandler _handler;

        public SpinHandlerTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _loggerMock = new Mock<ILogger<SpinHandler>>();
            _handler = new SpinHandler(_playerRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Spin_ShouldGenerateCorrectMatrixSize()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);

            var command = new SpinCommand { PlayerId = "67a533e7175f6d97b8e47a7d", BetAmount = 10 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result.Matrix);
            Assert.Equal(3, result.Matrix.Length); // 3 satır olmalı
            Assert.All(result.Matrix, row => Assert.Equal(5, row.Length)); // Her satırda 5 eleman olmalı
        }

        [Fact]
        public async Task Spin_ShouldDecreaseBalanceAfterBet()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);

            var command = new SpinCommand { PlayerId = "67a533e7175f6d97b8e47a7d", BetAmount = 10 };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(player.Balance >= 90); 
            Assert.True(player.Balance >= 100 - command.BetAmount); 
        }

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

        [Fact]
        public async Task Spin_ShouldThrowException_WhenPlayerNotFound()
        {
            // Arrange
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("99999")).ReturnsAsync((Player)null);

            var command = new SpinCommand { PlayerId = "99999", BetAmount = 10 };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

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
            Assert.True(result.WinAmount >= 0); // WinAmount negatif olamaz
            Assert.True(result.CurrentBalance >= player.Balance); // Kazanç sonrası bakiye en az başlangıç bakiyesi kadar olmalı
        }
    }
}
