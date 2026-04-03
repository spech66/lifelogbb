using LifelogBb.Models.Entities;

namespace LifelogBb.Models.Todos
{
    public class TodoIndexViewModel
    {
        public int TotalActive { get; set; }
        public int OverdueCount { get; set; }
        public int DueThisWeekCount { get; set; }
        public int CompletedCount { get; set; }
        public List<Todo> TodoItems { get; set; } = new();
        public List<Todo> InProgressItems { get; set; } = new();
        public List<Todo> DoneItems { get; set; } = new();
    }
}
