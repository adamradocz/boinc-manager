using BoincManager.Models;
using System.Collections.Generic;

namespace BoincManagerWindows.Models
{
    class HostGorup : BindableBase
    {
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }
        // The ObservableCollection refresh the list on item selected event, so when it happen it removes the selection
        public List<ObservableHost> Members { get; set; }

        public HostGorup(string name)
        {
            Name = name;
        }
    }
}
