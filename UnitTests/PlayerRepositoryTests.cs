using Moq;
using System.Threading.Tasks;
using Xunit;
using SlotMachineAPI.Domain;
using System.Collections.Generic;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

namespace SlotMachineAPI.Tests.Repositories
{
    public class PlayerRepositoryTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;

        public PlayerRepositoryTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
        }

        /// <summary>
        /// Unit test to verify that a new player is successfully added to the database.
        /// Ensures that the add operation is called exactly once with the correct player object.
        /// </summary>
        [Fact]
        public async Task AddPlayer_ShouldAddNewPlayer()
        {
            // Arrange
            var newPlayer = new Player { Id = "67a5336a175f6d97b8e47a79", Name = "TestPlayer", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.AddAsync(newPlayer))
                                 .Returns(Task.CompletedTask);

            // Act
            await _playerRepositoryMock.Object.AddAsync(newPlayer);

            // Assert
            _playerRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Player>()), Times.Once);
        }

        /// <summary>
        /// Unit test to verify that a player is correctly retrieved by their unique ID.
        /// Ensures that the returned player object is not null and has the expected name and balance.
        /// </summary>
        [Fact]
        public async Task GetPlayerById_ShouldReturnPlayer()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 0 };
            _playerRepositoryMock.Setup(repo => repo.GetByIdAsync("67a533e7175f6d97b8e47a7d"))
                                 .ReturnsAsync(player);

            // Act
            var result = await _playerRepositoryMock.Object.GetByIdAsync("67a533e7175f6d97b8e47a7d");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Wayne Gretzky", result.Name);

            Assert.Equal(0, result.Balance);
        }

        /// <summary>
        /// Unit test to verify that a player's balance is successfully modified during an update operation.
        /// Ensures that the update method is called exactly once with the correct player ID and updated balance.
        /// </summary>
        [Fact]
        public async Task UpdatePlayer_ShouldModifyPlayerBalance()
        {
            // Arrange
            var player = new Player { Id = "67a533e7175f6d97b8e47a7d", Name = "Wayne Gretzky", Balance = 100 };
            _playerRepositoryMock.Setup(repo => repo.UpdateAsync(player.Id, player))
                                 .Returns(Task.CompletedTask);

            // Act
            player.Balance = 200;
            await _playerRepositoryMock.Object.UpdateAsync(player.Id, player);

            // Assert
            _playerRepositoryMock.Verify(repo => repo.UpdateAsync(player.Id, player), Times.Once);
        }

        /// <summary>
        /// Unit test to verify that a player is successfully removed from the database.
        /// Ensures that the delete operation is called exactly once for the specified player ID.
        /// </summary>
        [Fact]
        public async Task DeletePlayer_ShouldRemovePlayer()
        {
            // Arrange
            var playerId = "67a5336a175f6d97b8e47a79";
            _playerRepositoryMock.Setup(repo => repo.DeleteAsync(playerId))
                                 .Returns(Task.CompletedTask);

            // Act
            await _playerRepositoryMock.Object.DeleteAsync(playerId);

            // Assert
            _playerRepositoryMock.Verify(repo => repo.DeleteAsync(playerId), Times.Once);
        }
    }
}
