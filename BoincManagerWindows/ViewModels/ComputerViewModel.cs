using BoincManager.Models;
using BoincManager.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BoincManagerWindows.ViewModels
{
    class ComputerViewModel : HostViewModel, IFilterableViewModel
    {
        private string name;
        public override string Name { get => name; protected set => SetProperty(ref name, value); }

        private string ipAddress;
        public override string IpAddress { get => ipAddress; protected set => SetProperty(ref ipAddress, value); }

        private int port;
        [Range(0, 65535, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public override int Port { get => port; protected set => SetProperty(ref port, value); }

        private string password;
        [Category("Security")]
        [PasswordPropertyText(true)]
        public override string Password { get => password; protected set => SetProperty(ref password, value); }

        private string boincVersion;
        public override string BoincVersion { get => boincVersion; protected set => SetProperty(ref boincVersion, value); }

        private string operatingSystem;
        public override string OperatingSystem { get => operatingSystem; protected set => SetProperty(ref operatingSystem, value); }
        
        private bool connected;
        public override bool Connected { get => connected; protected set => SetProperty(ref connected, value); }

        private string status;
        public override string Status { get => status; set => SetProperty(ref status, value); }

        public ComputerViewModel(Host hostModel) : base (hostModel)
        {
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return Name;
            yield return IpAddress;
            yield return BoincVersion;
            yield return OperatingSystem;
            yield return Status;
        }

        public static IEnumerable<string> GetLiveFilteringProperties()
        {
            yield return nameof(Name);
            yield return nameof(IpAddress);
            yield return nameof(BoincVersion);
            yield return nameof(OperatingSystem);
            yield return nameof(Status);
        }
    }
}
