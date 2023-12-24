namespace LifelogBb.Views.Shared.Components.Navigation
{
    public class NavItem
    {
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }

        public List<NavItem> SubItems { get; set; } = new List<NavItem>();

        public NavItem(string controller, string action, string title, string icon)
        {
            Controller = controller;
            Action = action;
            Title = title;
            Icon = icon;
        }

        public NavItem(string title, string icon, List<NavItem> subItems)
        {
            Title = title;
            Icon = icon;
            SubItems = subItems;
        }
    }
}
