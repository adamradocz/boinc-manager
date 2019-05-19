using BoincRpc;
using System;

namespace BoincManager.Models
{
    /// <summary>
    /// Contains all the data, the RpcClient and the BoincState of the connected Computer (host).
    /// </summary>
    public class HostState : IDisposable
    {
        bool disposed = false;

        public RpcClient RpcClient { get; }
        public BoincState BoincState { get; }

        public int Id { get; }
        public string ComputerName { get; set; }

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
        public bool Localhost { get; private set; }
        public bool Authorized { get; set; }

        public HostState(int id, string computerName)
        {
            RpcClient = new RpcClient();
            BoincState = new BoincState(RpcClient);

            Id = id;
            ComputerName = computerName;            
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
