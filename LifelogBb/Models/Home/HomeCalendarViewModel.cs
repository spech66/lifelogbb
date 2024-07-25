namespace LifelogBb.Models.Home
{
    public class HomeCalendarViewModelActivity
    {
        public string Type { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public DateTime? Date { get; set; }
    }

    public class HomeCalendarViewModel
    {
        public List<HomeCalendarViewModelActivity> Activities { get; set; } = new List<HomeCalendarViewModelActivity>();
    }
}
