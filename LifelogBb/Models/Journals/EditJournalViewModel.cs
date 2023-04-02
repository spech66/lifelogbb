namespace LifelogBb.Models.Journals
{
    public class EditJournalViewModel
    {
        public long Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? Category { get; set; }

        public string? Tags { get; set; }
    }
}
