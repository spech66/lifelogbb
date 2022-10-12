using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Weights
{
    public class WeightInput
    {
        [Range(40, 220)] // 100 cm => 39,3701 inch...
        public int Height { get; set; }

        [Range(40, 440)] // 200 Kg => 440 lbs...
        public int BodyWeight { get; set; }

        public decimal Bmi => ((BodyWeight * 1.0M) / (((Height * 0.01M) * Height) * 0.01M));
    }
}
