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

        public async Task AddHost(Host host)
        {
            var hostState = new HostState(host);

            if (hostState.AutoConnect)
            {
                await hostState.Connect();
            }
            
            HostsState.Add(host.Id, hostState);
        }

        public void RemoveHost(Host hostModel)
        {
            if (HostsState.ContainsKey(hostModel.Id))
            {
                HostsState[hostModel.Id].Close();
                HostsState[hostModel.Id].Dispose();
                HostsState.Remove(hostModel.Id);
            }
        }

        public void UpdateHost(Host host)
        {
            HostsState[host.Id].Update(host);
        }
    }
}
