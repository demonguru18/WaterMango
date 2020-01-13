using System;
using System.ComponentModel.DataAnnotations;

namespace WaterMangoApp.Model.BusinessModels
{
    public class PlantViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        
        [Required]
        public string ImageUrl { get; set; }
        
        public string Description { get; set; }
        public DateTime LastWateredAt { get; set; }
        
        public DateTime NextWateredAt { get; set; }
        
        public bool Status { get; set; }
        
        public int TotalWaterTime { get; set; }
        
        public string JobId { get; set; }
    }
}