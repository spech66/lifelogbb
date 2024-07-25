using LifelogBb.Utilities;

namespace LifelogBb.Models.Home
{
    public class CalendarViewModelEvent
    {
        public EntityType Type { get; set; } = EntityType.Unknown;

        public string Text { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class CalendarViewModel
    {
        public List<CalendarViewModelEvent> Events { get; set; } = new List<CalendarViewModelEvent>();

        public DateTime Date { get; set; } = DateTime.Now.Date;
    }
}
