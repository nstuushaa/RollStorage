using Microsoft.EntityFrameworkCore;
using RollStorage.Models;
using RollStorage.Repositories;
using System.Threading.Tasks;

namespace RollStorage.Services
{
    public class RollService
    {
        private readonly IRollRepository _rollRepository;

        public RollService(IRollRepository rollRepository)
        {
            _rollRepository = rollRepository;
        }
        public async Task<List<Roll>> GetAllRollsAsync()
        {
            return await _rollRepository.GetAllRollsAsync();
        }

        public async Task<Roll?> GetRollByIdAsync(int id)
        {
            return await _rollRepository.GetRollByIdAsync(id);
        }

        public async Task AddRollAsync(Roll roll)
        {
            await _rollRepository.AddRollAsync(roll);
        }

        public async Task UpdateRollAsync(Roll roll)
        {
            await _rollRepository.UpdateRollAsync(roll);
        }
        public async Task<RollStatisticsDto> GetStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var rolls = await _rollRepository.GetAllRollsAsync();

            return CalculateStatistic(rolls, startDate, endDate);
        }

        public RollStatisticsDto CalculateStatistic(List<Roll> rolls, DateTime startDate, DateTime endDate) 
        {
            try
            {
                var rollsInPeriod = rolls
                    .Where(r => r.AddedAt <= endDate && (r.RemoveAt == null || r.RemoveAt >= startDate))
                    .ToList();
                var addedCount = rolls
                    .Where(r => r.AddedAt >= startDate && r.AddedAt <= endDate).Count();
                var removedCount = rolls
                    .Where(r => r.RemoveAt.HasValue && r.RemoveAt >= startDate && r.RemoveAt <= endDate).Count();
                var averageLength = rollsInPeriod.Select(r => r.Length).DefaultIfEmpty(0).Average();
                var averageWeight = rollsInPeriod.Select(r => r.Weight).DefaultIfEmpty(0).Average();
                var maxLength = rollsInPeriod.Select(r => r.Length).DefaultIfEmpty(0).Max();
                var minLength = rollsInPeriod.Select(r => r.Length).DefaultIfEmpty(0).Min();
                var maxWeight = rollsInPeriod.Select(r => r.Weight).DefaultIfEmpty(0).Max();
                var minWeight = rollsInPeriod.Select(r => r.Weight).DefaultIfEmpty(0).Min();
                var totalWeight = rollsInPeriod.Select(r => r.Weight).DefaultIfEmpty(0).Sum();
                var durations = rollsInPeriod
                    .Where(r => r.RemoveAt.HasValue)
                    .Select(r => r.RemoveAt.Value - r.AddedAt)
                    .ToList();
                var maxDuration = durations.DefaultIfEmpty(TimeSpan.Zero).Max();
                var minDuration = durations.DefaultIfEmpty(TimeSpan.Zero).Min();


                var statistics = new RollStatisticsDto
                {
                    AddedCount = addedCount,
                    RemovedCount = removedCount,
                    AverageLenght = averageLength,
                    AverageWeight = averageWeight,
                    MaxLenght = maxLength,
                    MaxWeight = maxWeight,
                    MinLenght = minLength,
                    MinWeight = minWeight,
                    TotalWeight = totalWeight,
                    MaxDuration = maxDuration,
                    MinDuration = minDuration
                };
                return statistics;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при расчёте статистики", ex);
            }
        }
    }
}
