namespace RollStorage.Models
{
    public class RollFiltersDto
    {
        public int? MinId { get; set; }
        public int? MaxId { get; set; }
        public double? MinLength { get; set; }
        public double? MaxLength { get; set; }
        public double? MinWeight { get; set; }
        public double? MaxWeight { get; set; }
        public DateTime? StartAddedAt { get; set; }
        public DateTime? EndAddedAt { get; set; }
        public DateTime? StartRemoveAt { get; set; }
        public DateTime? EndRemoveAt { get; set; }
    }
}
