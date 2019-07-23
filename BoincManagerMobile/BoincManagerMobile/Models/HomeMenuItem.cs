namespace BoincManagerMobile.Models
{
    public enum MenuItemType
    {
        Hosts,
        Projects,
        Tasks,
        Transfers,
        Messages,
        Preferences,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
