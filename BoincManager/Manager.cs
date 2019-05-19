using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoincManager.Models;
using BoincManager.ViewModels;

namespace BoincManager
{
    public class Manager
    {
        public Dictionary<int, HostState> HostsState { get; } // Key is the host's Id
        public List<HostViewModel> HostsVm { get; }

        public Manager()
        {
            HostsVm = new List<HostViewModel>();
            HostsState = new Dictionary<int, HostState>();            
        }

        public async Task Initialize(IList<Host> hosts)
        {
            foreach (var host in hosts)
            {
                await AddHost(host);
            }
        }

        public async Task AddHost(Host host)
        {
            var hostVm = new HostViewModel(host);
            await Connect(hostVm);
            HostsVm.Add(hostVm);
        }

        public void RemoveHost(Host host)
        {
            // Remove the host from Models
            if (HostsState.ContainsKey(host.Id))
            {
                HostsState[host.Id].Close();
                HostsState[host.Id].Dispose();
                HostsState.Remove(host.Id);
            }            

            // Remove the host from ViewModels
            for (int i = 0; i < HostsVm.Count; i++)
            {
                if (HostsVm[i].Id == host.Id)
                {
                    HostsVm.RemoveAt(i);
                    break;
                }
            }            
        }

        public void UpdateHost(Host host)
        {
            for (int i = 0; i < HostsVm.Count; i++)
            {
                if (HostsVm[i].Id == host.Id)
                {
                    HostsVm[i].Update(host);
                    break;
                }
            }
        }

        public async Task Connect(HostViewModel hostVm)
        {
            if (string.IsNullOrWhiteSpace(hostVm.IpAddress) || string.IsNullOrEmpty(hostVm.Password) || hostVm.Connected)
            {
                return;
            }

            hostVm.Status = $"Connecting...";

            try
            {
                HostState hostState = new HostState(hostVm.Id, hostVm.Name);

                // Connecting to host
                await hostState.RpcClient.ConnectAsync(hostVm.IpAddress, hostVm.Port);
                hostState.Authorized = await hostState.RpcClient.AuthorizeAsync(hostVm.Password);

                if (hostState.Authorized)
                {
                    hostVm.Status = "Connected. Updating...";

                    HostsState.Add(hostVm.Id, hostState);

                    // Updating the hostState Model
                    await hostState.BoincState.UpdateAll();
                }
                else
                {
                    hostVm.Status = "Authorization error.";
                }

            }
            catch (Exception e)
            {
                hostVm.Status = $"Error connecting. {e.Message}";
            }
        }
    }
}
