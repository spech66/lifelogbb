using LifelogBb.Interfaces.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.Models.Entities
{
    public class Weight : BaseEntity
    {
        [Range(40, 220)] // 100 cm => 39,3701 inch...
        public int Height { get; set; }

        [DisplayName("Weight")]
        [Range(40, 440)] // 200 Kg => 440 lbs...
        [Column(TypeName = "decimal(18, 1)")]
        public decimal BodyWeight { get; set; }

        [DisplayName("BMI")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Bmi { get; set; }

        [DisplayName("BMI Overweight")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BmiOverweight => 25.0M;

        [DisplayName("BMI Underweight")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal BmiUnderweight => 18.5M;
    }
}
