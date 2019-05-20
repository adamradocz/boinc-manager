using BoincManager.Models;
using System.Threading.Tasks;

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

        public async Task FirstUpdateOnConnect(HostState hostState)
        {
            OperatingSystem = hostState.BoincState.CoreClientState.HostInfo.OSName;
            BoincVersion = $"{hostState.BoincState.CoreClientState.CoreClientMajorVersion}.{hostState.BoincState.CoreClientState.CoreClientMinorVersion}.{hostState.BoincState.CoreClientState.CoreClientReleaseVersion}";
            Connected = true;
            Status = await Statuses.GetHostStatus(hostState);
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
