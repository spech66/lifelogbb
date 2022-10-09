using System.ComponentModel.DataAnnotations;

namespace LifelogBb.Models
{
    public class BaseEntity
    {
        public long Id { get; set; }

        [Display(Name = "Creation Date")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Update Date")]
        [DataType(DataType.Date)]
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
