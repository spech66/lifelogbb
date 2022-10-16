using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LifelogBb.Models.Weights
{
    public class EditWeightViewModel
    {
        public long Id { get; set; }

        [Range(40, 220)] // 100 cm => 39,3701 inch...
        public int Height { get; set; }

        [DisplayName("Weight")]
        [Range(40.0, 440.0)] // 200 Kg => 440 lbs...
        public double BodyWeight { get; set; }
    }
}
