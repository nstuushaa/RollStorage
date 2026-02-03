using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RollStorage.Models
{
    public class Roll
    {
        public int Id { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "Длина должна быть больше 0")]
        public double Length { get; set; }

        [Range(0.01, 10000000, ErrorMessage = "Вес должен быть больше 0")]
        public double Weight { get; set; }
        public DateTime AddedAt { get; set; }

        public DateTime? RemoveAt { get; set; }

    }
}
