using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class BaseEntityTagged  : BaseEntity
    {
        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
