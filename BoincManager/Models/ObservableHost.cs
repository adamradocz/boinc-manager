using BoincManager.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BoincManager.Models
{
    public class ObservableHost : BindableBase, IHost, IFilterable, IEquatable<ObservableHost>
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

        #region Equality comparisons
        /* From:
         * - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
         * - https://intellitect.com/overidingobjectusingtuple/
         * - https://montemagno.com/optimizing-c-struct-equality-with-iequatable/
        */

        public bool Equals(ObservableHost other)
        {
            // If parameter is null, return false.
            if (other is null)
                return false;

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            ObservableHost host = obj as ObservableHost;
            return host == null ? false : Equals(host);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ObservableHost lhs, ObservableHost rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ObservableHost lhs, ObservableHost rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
