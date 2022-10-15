using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models.Entities
{
    public class BaseEntity
    {
        public long Id { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Update Date")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public void SetCreateFields()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetUpdateFields()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
