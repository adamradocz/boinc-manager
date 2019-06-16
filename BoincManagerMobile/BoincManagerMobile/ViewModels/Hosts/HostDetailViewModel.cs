using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class HostDetailViewModel : BaseViewModel
    {
        public Host Host { get; set; }
        public HostDetailViewModel(Host host)
        {
            Title = host.Name;
            Host = host;
        }
    }
}
