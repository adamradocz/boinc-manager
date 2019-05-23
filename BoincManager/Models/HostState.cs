using BoincRpc;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BoincManager.Models
{
    /// <summary>
    /// Contains all the data, the RpcClient and the BoincState of the connected host.
    /// </summary>
    public class HostState : IDisposable
    {
        bool disposed = false;

        public int Id { get; }
        public string Name { get; set; }

        private string ipAddress;
        public string IpAddress
        {
            get => ipAddress;
            set
            {
                ipAddress = value;
                Localhost = ipAddress.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || ipAddress.Equals("127.0.0.1", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public int Port { get; set; }
        public string Password { get; set; }
        public bool AutoConnect { get; set; }
        public bool Localhost { get; private set; }
        public bool Authorized { get; set; }
        public string BoincVersion => Connected
                    ? $"{BoincState.CoreClientState.CoreClientMajorVersion}.{BoincState.CoreClientState.CoreClientMinorVersion}.{BoincState.CoreClientState.CoreClientReleaseVersion}"
                    : string.Empty;
        public string OperatingSystem => Connected ? BoincState.CoreClientState.HostInfo.OSName : string.Empty;
        public bool Connected { get; set; }
        public string Status { get; set; }

        public RpcClient RpcClient { get; }
        public BoincState BoincState { get; }

        public HostState(Host host)
        {
            Id = host.Id;
            Update(host);

            RpcClient = new RpcClient();
            BoincState = new BoincState(RpcClient);
        }

        public void Update(Host host)
        {
            Name = host.Name;
            IpAddress = host.IpAddress;
            Port = host.Port;
            Password = host.Password;
            AutoConnect = host.AutoConnect;
        }

        public async Task<string> GetHostStatus()
        {
            StringBuilder status = new StringBuilder();
            status.Append("Connected. ");

            string newerVersion = await RpcClient.GetNewerVersionAsync();
            if (!string.IsNullOrEmpty(newerVersion))
                status.Append($"BOINC {newerVersion} is available for download on {Name}.");

            return status.ToString().TrimEnd(' ');
        }

        public void Close()
        {
            RpcClient.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (RpcClient != null)
                {
                    RpcClient.Dispose();
                }
            }

            disposed = true;
        }
    }
}
