using LifelogBb.Interfaces.DTOs;

namespace LifelogBb.ApiDTOs.Journals
{
    public class JournalOutput : IBaseOutput
    {
        public long Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
