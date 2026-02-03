namespace RollStorage.Models
{
    public class RollStatisticsDto
    {
        public int AddedCount { get; set; }
        public int RemovedCount { get; set; }
        public double AverageLenght { get; set; }
        public double AverageWeight { get; set; }
        public double MaxLenght { get; set; }
        public double MinLenght { get; set; }
        public double MaxWeight { get; set; }
        public double MinWeight { get; set; }
        public double TotalWeight { get; set; }
        public TimeSpan MaxDuration { get; set; }
        public TimeSpan MinDuration { get; set; }
        public DateTime DayOfMinRolls { get; set; }
        public DateTime DayOfMaxRolls { get; set; }
        public DateTime DayOfMinSumWeight { get; set; }
        public DateTime DayOfMaxSumWeight { get; set; }
    }
}
