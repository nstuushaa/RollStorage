using System.ComponentModel.DataAnnotations;

namespace RollStorage.Models
{
    public class CreateRollDto
    {
        [Required]
        [Range(0.01, 10000000, ErrorMessage = "Длина должна быть больше 0")]
        public double Length { get; set; }

        [Required]
        [Range(0.01, 10000000, ErrorMessage = "Длина должна быть больше 0")]
        public double Weight { get; set; }
    }
}
