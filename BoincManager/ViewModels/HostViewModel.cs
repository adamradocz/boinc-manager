using BoincManager.Models;
using System.Threading.Tasks;

namespace BoincManager.ViewModels
{
    public class HostViewModel : BindableBase
    {
        public int Id { get; }
        public virtual string Name { get; protected set; }
        public virtual string IpAddress { get; protected set; }
        public virtual int Port { get; protected set; }
        public virtual string Password { get; protected set; }
        public virtual string BoincVersion { get; protected set; }
        public virtual string OperatingSystem { get; protected set; }
        public virtual bool Connected { get; protected set; }
        public virtual string Status { get; set; }
        
        public HostViewModel(Host  hostModel)
        {
            Id = hostModel.Id;
            Name = hostModel.Name;
            IpAddress = hostModel.IpAddress;
            Port = hostModel.Port;
            Password = hostModel.Password;
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
