using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.DTOs.Weights
{
    public class WeightOutput : IBaseOutput
    {
        public long Id { get; set; }

        public int Height { get; set; }

        public int BodyWeight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public decimal Bmi { get; set; }

        public decimal BmiOverweight => 25.0M;

        public decimal BmiUnderweight => 18.5M;
    }
}
