namespace RollStorage.Models
{
    public class Roll
    {
        public int Id { get; set; }
        public double Length { get; set; }
        public double Weight { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime? RemoveAt { get; set; }

    }
}
