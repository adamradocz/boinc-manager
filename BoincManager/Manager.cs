using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoincManager.Models;

namespace BoincManager
{
    public class Manager
    {
        public Dictionary<int, HostState> HostsState { get; } // Key is the host's Id

        public Manager()
        {
            HostsState = new Dictionary<int, HostState>();
        }

        public async Task Start(IList<Host> hosts)
        {
            // Connect to all the hosts stored in the database.
            foreach (var host in hosts)
            {
                await AddHost(host);
            }
            // await Utils.ParallelForEachAsync(hosts, AddHost); // Connect to all the hosts in parallel, instead of sequential order
        }

        public async Task Connect(HostState hostState)
        {
            if (string.IsNullOrWhiteSpace(hostState.IpAddress) || string.IsNullOrEmpty(hostState.Password) || hostState.Connected)
            {
                return;
            }

            hostState.Status = $"Connecting...";

            try
            {
                // Connecting to host
                await hostState.RpcClient.ConnectAsync(hostState.IpAddress, hostState.Port);
                hostState.Authorized = await hostState.RpcClient.AuthorizeAsync(hostState.Password);

                if (hostState.Authorized)
                {
                    hostState.Connected = true;
                    hostState.Status = "Connected. Updating...";

                    await hostState.BoincState.UpdateAll();
                    hostState.Status = await hostState.GetHostStatus();
                }
                else
                {
                    hostState.Status = "Authorization error.";
                }

            }
            catch (Exception e)
            {
                hostState.Status = $"Error connecting. {e.Message}";
            }
        }

        public void Disconnect(HostState hostState)
        {
            if (!hostState.Connected)
            {
                return;
            }

            hostState.Status = string.Empty;

            if (HostsState.ContainsKey(hostState.Id))
            {
                HostsState[hostState.Id].Close();
                HostsState[hostState.Id].Connected = false;
            }
        }

        public async Task AddHost(Host host)
        {
            var hostState = new HostState(host);

            if (hostState.AutoConnect)
            {
                await Connect(hostState);
            }
            
            HostsState.Add(host.Id, hostState);
        }

        public void RemoveHost(int hostId)
        {
            if (HostsState.ContainsKey(hostId))
            {
                HostsState[hostId].Close();
                HostsState[hostId].Dispose();
                HostsState.Remove(hostId);
            }
        }

        public void UpdateHost(Host host)
        {
            HostsState[host.Id].Update(host);
        }
    }
}
