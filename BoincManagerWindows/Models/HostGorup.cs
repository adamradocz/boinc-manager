using BoincManager.Models;
using BoincManagerWindows.Models;
using System.Collections.ObjectModel;

namespace BoincManagerWindows.Models
{
    class HostGorup : BindableBase
    {
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }
        public ObservableCollection<Host> Members { get; set; }

        public HostGorup(string name)
        {
            Name = name;
        }
    }
}
