using BoincManager.Models;
using System.Threading.Tasks;

namespace BoincManager.ViewModels
{
    public class HostViewModel
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual int Port { get; set; }
        public virtual string Password { get; set; }
        public virtual string BoincVersion { get; set; }
        public virtual string OperatingSystem { get; set; }
        public virtual bool Connected { get; set; }
        public virtual string Status { get; set; }
        
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
