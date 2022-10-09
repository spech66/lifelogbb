using LifelogBb.Interfaces.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Weight
{
    public class Weight : BaseEntity
    {
        [Range(40, 220)] // 100 cm => 39,3701 inch...
        public int Height { get; set; }

        [Range(40, 440)] // 200 Kg => 440 lbs...
        public int BodyWeight { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Bmi { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BmiOverweight => 25.0M;
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BmiUnderweight => 18.5M;
    }
}
