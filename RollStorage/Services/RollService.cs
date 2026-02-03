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
        public async Task<List<Roll>> GetAllRollsAsync(RollFiltersDto filter)
        {
            if (filter.MinId.HasValue && filter.MaxId.HasValue && filter.MinId > filter.MaxId)
                throw new ArgumentException("MinId не может быть больше MaxId");
            if (filter.MinLength.HasValue && filter.MaxLength.HasValue && filter.MinLength > filter.MaxLength)
                throw new ArgumentException("MinLength не может быть больше MaxLength");
            if (filter.MinWeight.HasValue && filter.MaxWeight.HasValue && filter.MinWeight > filter.MaxWeight)
                throw new ArgumentException("MinWeight не может быть больше MaxWeight");
            if (filter.StartAddedAt.HasValue && filter.EndAddedAt.HasValue && filter.StartAddedAt > filter.EndAddedAt)
                throw new ArgumentException("StartAddedAt не может быть позже EndAddedAt");
            if (filter.StartRemoveAt.HasValue && filter.EndRemoveAt.HasValue && filter.StartRemoveAt > filter.EndRemoveAt)
                throw new ArgumentException("StartRemoveAt не может быть позже EndRemoveAt");

            var rolls = await _rollRepository.GetAllRollsAsync();           

            if (filter.MinId.HasValue)
                rolls = rolls.Where(r => r.Id >= filter.MinId.Value).ToList();
            if (filter.MaxId.HasValue)
                rolls = rolls.Where(r => r.Id <= filter.MaxId.Value).ToList();

            if (filter.MinLength.HasValue)
                rolls = rolls.Where(r => r.Length >= filter.MinLength.Value).ToList();
            if (filter.MaxLength.HasValue)
                rolls = rolls.Where(r => r.Length <= filter.MaxLength.Value).ToList();

            if (filter.MinWeight.HasValue)
                rolls = rolls.Where(r => r.Weight >= filter.MinWeight.Value).ToList();
            if (filter.MaxWeight.HasValue)
                rolls = rolls.Where(r => r.Weight <= filter.MaxWeight.Value).ToList();

            if (filter.StartAddedAt.HasValue)
                rolls = rolls.Where(r => r.AddedAt >= filter.StartAddedAt.Value).ToList();
            if (filter.EndAddedAt.HasValue)
                rolls = rolls.Where(r => r.AddedAt <= filter.EndAddedAt.Value).ToList();

            if (filter.StartRemoveAt.HasValue)
                rolls = rolls.Where(r => r.RemoveAt >= filter.StartRemoveAt.Value).ToList();
            if (filter.EndRemoveAt.HasValue)
                rolls = rolls.Where(r => r.RemoveAt <= filter.EndRemoveAt.Value).ToList();

            if (!rolls.Any())
                throw new KeyNotFoundException("Рулоны не найдены");
            return rolls;
        }

        public async Task<Roll> GetRollByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id должен быть больше 0");

            var roll = await _rollRepository.GetRollByIdAsync(id);

            if (roll == null)
                throw new KeyNotFoundException("Рулон не найден");

            return roll;
        }

        public async Task<Roll> AddRollAsync(CreateRollDto dto)
        {
            var roll = new Roll
            {
                Length = dto.Length,
                Weight = dto.Weight,
                AddedAt = DateTime.UtcNow
            };

            await _rollRepository.AddRollAsync(roll);
            return roll;
        }

        public async Task<Roll> RemoveRollAsync(int id, DateTime removeAt)
        {
            var roll = await _rollRepository.GetRollByIdAsync(id);
            if (roll == null)
                throw new KeyNotFoundException("Рулон не найден");
            if(roll.RemoveAt != null)
                throw new InvalidOperationException("Рулон уже удалён со склада");
            roll.RemoveAt = removeAt;

            await _rollRepository.UpdateRollAsync(roll);

            return roll;
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

                var dayStatsList = new List<DayStatistics>();

                for (DateTime currentDay = startDate; currentDay <= endDate; currentDay = currentDay.AddDays(1)) 
                {
                    var rollsOnDay = rollsInPeriod
                        .Where(r => r.AddedAt <= currentDay && (r.RemoveAt == null || r.RemoveAt >= currentDay)).ToList();
                    dayStatsList.Add(new DayStatistics
                    {
                        Date = currentDay,
                        RollCount = rollsOnDay.Count,
                        TotalWeight = rollsOnDay.Sum(r => r.Weight)
                    });
                }

                var dayOfMinRolls = dayStatsList
                    .OrderBy(d => d.RollCount)
                    .First();
                var dayOfMaxRolls = dayStatsList
                    .OrderByDescending(d => d.RollCount)
                    .First();
                var dayOfMinWeight = dayStatsList
                    .OrderBy(d => d.TotalWeight)
                    .First();
                var dayOfMaxWeight = dayStatsList
                    .OrderByDescending(d => d.TotalWeight)
                    .First();

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
                    MinDuration = minDuration,
                    DayOfMinRolls = dayOfMinRolls.Date,
                    DayOfMaxRolls= dayOfMaxRolls.Date,
                    DayOfMinSumWeight = dayOfMinWeight.Date,
                    DayOfMaxSumWeight = dayOfMaxWeight.Date

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
