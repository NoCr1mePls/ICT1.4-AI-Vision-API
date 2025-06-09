using System.ComponentModel.DataAnnotations;

namespace HomeTry.Models
{
    public class Litter
    {
        [Key]
        public Guid litter_id { get; set; }

        [Required]
        public int litter_classification { get; set; }

        [Range(0.85, 1)]
        public float confidence { get; set; }

        [Required]
        public float location_latitude { get; set; }

        [Required]
        public float location_longitude { get; set; }

        [Required]
        public DateTime detection_time { get; set; }

        public Guid? weather_id { get; set; }

        [Required]
        public Weather weather { get; set; }
    }
}
