using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifelogBb.ApiDTOs.Weights
{
    public class WeightInput
    {
        [Range(40, 220)] // 100 cm => 39,3701 inch...
        public int Height { get; set; }

        [Range(40, 440)] // 200 Kg => 440 lbs...
        public double BodyWeight { get; set; }

        public double Bmi => ((BodyWeight * 1.0) / (((Height * 0.01) * Height) * 0.01));
    }
}
