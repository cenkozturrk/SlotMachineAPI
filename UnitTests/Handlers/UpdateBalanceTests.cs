using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using SlotMachineAPI.Domain;
using Microsoft.Extensions.Logging;
using SlotMachineAPI.Application.Players.Commands.UpdatePlayerCommand;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

namespace SlotMachineAPI.Tests.Handlers
{
    public class UpdateBalanceTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;
        private readonly Mock<ILogger<UpdateBalanceHandler>> _loggerMock;
        private readonly UpdateBalanceHandler _handler;

        public UpdateBalanceTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
            _loggerMock = new Mock<ILogger<UpdateBalanceHandler>>();
            _handler = new UpdateBalanceHandler(_playerRepositoryMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Unit test to verify that a player's balance correctly increases when a positive amount is applied.
        /// Ensures that the balance is updated correctly and that the update operation is performed once.
        /// </summary>
        [Fact]
        public async Task UpdateBalance_ShouldIncreaseBalance()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);

            var command = new UpdateBalanceCommand { PlayerId = "67a533e7175f6d97b8e47a7d", Amount = 50 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(150, player.Balance);
            _playerRepositoryMock.Verify(repo => repo.UpdateAsync(player.Id, player), Times.Once);
        }

        /// <summary>
        /// Unit test to verify that a player's balance correctly decreases when a negative amount is applied.
        /// Ensures that the balance is reduced by the specified amount and that the update operation is performed once.
        /// </summary>
        [Fact]
        public async Task UpdateBalance_ShouldDecreaseBalance()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player)).Returns(Task.CompletedTask);

            var command = new UpdateBalanceCommand { PlayerId = "67a533e7175f6d97b8e47a7d", Amount = -30 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Equal(70, player.Balance);
            _playerRepositoryMock.Verify(repo => repo.UpdateAsync(player.Id, player), Times.Once);
        }

        /// <summary>
        /// Unit test to verify that an InvalidOperationException is thrown when attempting to update a player's balance below zero.
        /// Ensures that the balance remains unchanged and no update operation is performed if the balance would become negative.
        /// </summary>
        [Fact]
        public async Task UpdateBalance_ShouldNotAllowNegativeBalance()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 20 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d")).ReturnsAsync(player);

            var command = new UpdateBalanceCommand { PlayerId = "67a533e7175f6d97b8e47a7d", Amount = -50 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
            _playerRepositoryMock.Verify(repo => repo.UpdateAsync(player.Id, player), Times.Never);
        }

        /// <summary>
        /// Unit test to verify that a KeyNotFoundException is thrown when attempting to update the balance of a non-existent player.
        /// Ensures that no update operation is performed if the player is not found in the database.
        /// </summary>
        [Fact]
        public async Task UpdateBalance_ShouldThrowException_WhenPlayerNotFound()
        {
            // Arrange
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("99999")).ReturnsAsync((Player)null);
            var command = new UpdateBalanceCommand { PlayerId = "99999", Amount = 50 };

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
            _playerRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<string>(), It.IsAny<Player>()), Times.Never);
        }
    }
}
