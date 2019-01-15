using System.Collections.ObjectModel;

namespace BoincManagerWindows.ViewModels
{
    class ComputerGorupViewModel : ViewModel
    {
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }
        public ObservableCollection<ComputerViewModel> Members { get; set; }

        public ComputerGorupViewModel(string name)
        {
            Name = name;
        }
    }
}
