using System.Collections.ObjectModel;

namespace BoincManagerWindows.ViewModels
{
    class ComputerGorupViewModel : BindableBase
    {
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }
        public ObservableCollection<HostViewModel> Members { get; set; }

        public ComputerGorupViewModel(string name)
        {
            Name = name;
        }
    }
}
