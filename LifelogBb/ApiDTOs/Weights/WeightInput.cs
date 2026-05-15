using System.ComponentModel.DataAnnotations;

namespace LifelogBb.ApiDTOs.Weights
{
    public class WeightInput
    {
        [Range(40, 440)] // 200 Kg => 440 lbs...
        public double BodyWeight { get; set; }
    }
}
