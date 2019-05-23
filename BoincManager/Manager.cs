using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoincManager.Models;

namespace BoincManager
{
    public class Manager
    {
        // Key is the host's Id
        private readonly Dictionary<int, HostState> _hostStates;

        public Manager()
        {
            _hostStates = new Dictionary<int, HostState>();
        }

        public HostState GetHostState(int id)
        {
            return _hostStates[id];
        }

        public IEnumerable<HostState> GetAllHostState()
        {
            return _hostStates.Values;
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

        public async Task Connect(int hostId)
        {

            if (string.IsNullOrWhiteSpace(_hostStates[hostId].IpAddress) || string.IsNullOrEmpty(_hostStates[hostId].Password) || _hostStates[hostId].Connected)
            {
                return;
            }

            _hostStates[hostId].Status = $"Connecting...";

            try
            {
                // Connecting to host
                await _hostStates[hostId].RpcClient.ConnectAsync(_hostStates[hostId].IpAddress, _hostStates[hostId].Port);
                _hostStates[hostId].Authorized = await _hostStates[hostId].RpcClient.AuthorizeAsync(_hostStates[hostId].Password);

                if (_hostStates[hostId].Authorized)
                {
                    _hostStates[hostId].Connected = true;
                    _hostStates[hostId].Status = "Connected. Updating...";

                    await _hostStates[hostId].BoincState.UpdateAll();
                    _hostStates[hostId].Status = await _hostStates[hostId].GetHostStatus();
                }
                else
                {
                    _hostStates[hostId].Status = "Authorization error.";
                }

            }
            catch (Exception e)
            {
                _hostStates[hostId].Status = $"Error connecting. {e.Message}";
            }
        }

        public void Disconnect(int hostId)
        {
            if (_hostStates.ContainsKey(hostId) && _hostStates[hostId].Connected)
            {
                _hostStates[hostId].Status = string.Empty;
                _hostStates[hostId].Close();
                _hostStates[hostId].Connected = false;
            }            
        }

        public async Task AddHost(Host host)
        {
            var hostState = new HostState(host);
            _hostStates.Add(host.Id, hostState);

            if (hostState.AutoConnect)
            {
                await Connect(hostState.Id);
            }            
        }

        public void RemoveHost(int hostId)
        {
            if (_hostStates.ContainsKey(hostId))
            {
                _hostStates[hostId].Close();
                _hostStates[hostId].Dispose();
                _hostStates.Remove(hostId);
            }
        }

        public void UpdateHost(Host host)
        {
            _hostStates[host.Id].Update(host);
        }
    }
}
