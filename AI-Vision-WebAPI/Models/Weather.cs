using System.ComponentModel.DataAnnotations;

namespace HomeTry.Models
{
    public class Weather
    {
        [Key]
        public Guid weather_id { get; set; }

        [Required]
        public double temperature_celsius { get; set; }

        [Required]
        public double humidity { get; set; }

        [Required, MaxLength(20)]
        public string conditions { get; set; }
    }
}
