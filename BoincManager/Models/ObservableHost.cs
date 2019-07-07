using BoincManager.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoincManager.Models
{
    public class ObservableHost : BindableBase, IHost, IFilterable
    {
        public int Id { get; }

        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }

        private string ipAddress;
        public string IpAddress { get => ipAddress; set => SetProperty(ref ipAddress, value); }

        private int port;
        [Range(0, 65535, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Port { get => port; set => SetProperty(ref port, value); }

        private string password;
        public string Password { get => password; set => SetProperty(ref password, value); }

        private bool autoConnect;
        public bool AutoConnect { get => autoConnect; set => SetProperty(ref autoConnect, value); }

        private string boincVersion;
        public string BoincVersion { get => boincVersion; private set => SetProperty(ref boincVersion, value); }

        private string operatingSystem;
        public string OperatingSystem { get => operatingSystem; private set => SetProperty(ref operatingSystem, value); }

        private bool connected;
        public bool Connected { get => connected; private set => SetProperty(ref connected, value); }

        private string status;
        public string Status { get => status; set => SetProperty(ref status, value); }

        public ObservableHost(HostState hostState)
        {
            Id = hostState.Id;
            Update(hostState);
        }

        public void Update(HostState hostState)
        {
            Name = hostState.Name;
            IpAddress = hostState.IpAddress;
            Port = hostState.Port;
            Password = hostState.Password;
            AutoConnect = hostState.AutoConnect;

            BoincVersion = hostState.BoincVersion;
            OperatingSystem = hostState.OperatingSystem;
            Connected = hostState.Connected;
            Status = hostState.Status;
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return Name;
            yield return IpAddress;
            yield return BoincVersion;
            yield return OperatingSystem;
            yield return Status;
        }
    }
}
