using System;
using System.Collections.Generic;
using System.Text;

namespace BoincManagerMobile.Models
{
    public enum MenuItemType
    {
        Hosts,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
