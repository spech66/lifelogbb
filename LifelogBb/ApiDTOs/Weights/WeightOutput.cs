using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.Weights
{
    public class WeightOutput : IBaseOutput
    {
        public long Id { get; set; }

        public int Height { get; set; }

        public double BodyWeight { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public double Bmi { get; set; }

        public double BmiOverweight => 25.0;

        public double BmiUnderweight => 18.5;
    }
}
