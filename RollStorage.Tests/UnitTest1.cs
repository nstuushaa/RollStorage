using Xunit;
using Moq;
using RollStorage.Services;
using RollStorage.Repositories;
using RollStorage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RollStorage.Tests
{
    public class RollServiceTests
    {
        private readonly Mock<IRollRepository> _mockRepo;
        private readonly RollService _service;

        public RollServiceTests()
        {
            _mockRepo = new Mock<IRollRepository>();
            _service = new RollService(_mockRepo.Object);
        }
        // Тесты для метода GetRollByID
        [Fact]
        public async Task GetRollByIdAsync_IdLessOrEqualZero_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetRollByIdAsync(0));
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetRollByIdAsync(-1));
        }

        [Fact]
        public async Task GetRollByIdAsync_RollNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetRollByIdAsync(1)).ReturnsAsync((Roll)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetRollByIdAsync(1));
        }

        [Fact]
        public async Task GetRollByIdAsync_RollExists_ReturnsRoll()
        {
            var roll = new Roll { Id = 1, Length = 10, Weight = 5 };
            _mockRepo.Setup(r => r.GetRollByIdAsync(1)).ReturnsAsync(roll);

            var result = await _service.GetRollByIdAsync(1);

            Assert.Equal(roll, result);
        }

        //Тесты для метода AddRollAsync
        [Fact]
        public async Task AddRollAsync_ValidData_ReturnsRollWithAddedAt()
        {
            var dto = new CreateRollDto { Length = 10, Weight = 5 };

            _mockRepo.Setup(r => r.AddRollAsync(It.IsAny<Roll>())).Returns(Task.CompletedTask);

            var result = await _service.AddRollAsync(dto);

            Assert.Equal(dto.Length, result.Length);
            Assert.Equal(dto.Weight, result.Weight);
            Assert.True(result.AddedAt <= DateTime.UtcNow);
            _mockRepo.Verify(r => r.AddRollAsync(It.IsAny<Roll>()), Times.Once);
        }

        // Тесты для метода RemoveRollAsync
        [Fact]
        public async Task RemoveRollAsync_RollNotFound_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetRollByIdAsync(1)).ReturnsAsync((Roll)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.RemoveRollAsync(1, DateTime.UtcNow));
        }

        [Fact]
        public async Task RemoveRollAsync_AlreadyRemoved_ThrowsInvalidOperationException()
        {
            var roll = new Roll { Id = 1, RemoveAt = DateTime.UtcNow };
            _mockRepo.Setup(r => r.GetRollByIdAsync(1)).ReturnsAsync(roll);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoveRollAsync(1, DateTime.UtcNow));
        }

        [Fact]
        public async Task RemoveRollAsync_ValidRoll_UpdatesRemoveAt()
        {
            var roll = new Roll { Id = 1, RemoveAt = null };
            _mockRepo.Setup(r => r.GetRollByIdAsync(1)).ReturnsAsync(roll);
            _mockRepo.Setup(r => r.UpdateRollAsync(roll)).Returns(Task.CompletedTask);

            var removeAt = DateTime.UtcNow;
            var result = await _service.RemoveRollAsync(1, removeAt);

            Assert.Equal(removeAt, result.RemoveAt);
            _mockRepo.Verify(r => r.UpdateRollAsync(roll), Times.Once);
        }

        // Тесты для метода GetAllRollAsync
        [Fact]
        public async Task GetAllRollsAsync_NoRolls_ThrowsKeyNotFoundException()
        {
            _mockRepo.Setup(r => r.GetAllRollsAsync()).ReturnsAsync(new List<Roll>());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetAllRollsAsync(new RollFiltersDto()));
        }

        [Fact]
        public async Task GetAllRollsAsync_WithFilters_ReturnsFilteredRolls()
        {
            var rolls = new List<Roll>
        {
            new Roll { Id = 1, Length = 10, Weight = 5 },
            new Roll { Id = 2, Length = 20, Weight = 15 }
        };
            _mockRepo.Setup(r => r.GetAllRollsAsync()).ReturnsAsync(rolls);

            var filter = new RollFiltersDto { MinId = 2 };
            var result = await _service.GetAllRollsAsync(filter);

            Assert.Single(result);
            Assert.Equal(2, result[0].Id);
        }

        [Fact]
        public async Task GetAllRollsAsync_MinGreaterThanMax_ThrowsArgumentException()
        {
            var filter = new RollFiltersDto { MinId = 5, MaxId = 2 };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetAllRollsAsync(filter));
        }

        // Тесты для метода GetStatisticsAsync
        [Fact]
        public async Task GetStatisticsAsync_ReturnsCorrectStatistics()
        {
            var rolls = new List<Roll>
        {
            new Roll { Id = 1, Length = 10, Weight = 5, AddedAt = DateTime.UtcNow.AddDays(-2), RemoveAt = DateTime.UtcNow.AddDays(-1) },
            new Roll { Id = 2, Length = 20, Weight = 10, AddedAt = DateTime.UtcNow.AddDays(-1), RemoveAt = null }
        };
            _mockRepo.Setup(r => r.GetAllRollsAsync()).ReturnsAsync(rolls);

            var start = DateTime.UtcNow.AddDays(-3);
            var end = DateTime.UtcNow;

            var result = await _service.GetStatisticsAsync(start, end);

            Assert.Equal(2, result.AddedCount);
            Assert.Equal(1, result.RemovedCount);
            Assert.Equal(15, result.AverageLenght);
            Assert.Equal(7.5, result.AverageWeight);
            Assert.Equal(20, result.MaxLenght);
            Assert.Equal(10, result.MaxWeight);
            Assert.Equal(10, result.MinLenght);
            Assert.Equal(5, result.MinWeight);
            Assert.True(result.TotalWeight > 0);
        }
    }
}
