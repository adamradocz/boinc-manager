using BoincManager.Models;

namespace BoincManager.ViewModels
{
    public class HostViewModel : Host
    {
        public string BoincVersion { get; set; }
        public string OperatingSystem { get; set; }
        public bool Connected { get; set; }
        public string Status { get; set; }
        
        public HostViewModel(Host host)
        {
            Id = host.Id;
            Name = host.Name;
            IpAddress = host.IpAddress;
            Port = host.Port;
            Password = host.Password;
        }

        public void Update(Host host)
        {
            Name = host.Name;
            IpAddress = host.IpAddress;
            Port = host.Port;
            Password = host.Password;
        }
    }
}
