using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LifelogBb.Models.Weights
{
    public class EditWeightViewModel
    {
        public long Id { get; set; }

        [DisplayName("Weight")]
        [Range(40.0, 440.0)] // 200 Kg => 440 lbs...
        public double BodyWeight { get; set; }
    }
}
