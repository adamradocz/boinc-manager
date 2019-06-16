using BoincManager.Models;

namespace BoincManagerMobile.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Host Host { get; set; }
        public ItemDetailViewModel(Host host = null)
        {
            Title = host?.Name;
            Host = host;
        }
    }
}
