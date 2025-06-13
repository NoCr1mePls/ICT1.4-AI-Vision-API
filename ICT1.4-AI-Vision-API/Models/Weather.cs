using System;
using System.ComponentModel.DataAnnotations;

namespace ICT1._4_AI_Vision_API.Models
{
    public class Weather
    {
        [Key]
        public Guid weather_id { get; set; }  

        [Required]
        public double temperature_celsius { get; set; }

        [Required]
        public double humidity { get; set; }

        [Required]
        public string conditions { get; set; }

        public Litter? Litter { get; set; }
    }
}
