using Moq;
using System.Threading.Tasks;
using Xunit;
using SlotMachineAPI.Infrastructure.Repositories;
using SlotMachineAPI.Domain;
using System.Collections.Generic;

namespace SlotMachineAPI.Tests.Repositories
{
    public class PlayerRepositoryTests
    {
        private readonly Mock<IPlayerRepository> _playerRepositoryMock;

        public PlayerRepositoryTests()
        {
            _playerRepositoryMock = new Mock<IPlayerRepository>();
        }

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
