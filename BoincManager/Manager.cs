﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BoincManager.Models;

namespace BoincManager
{
    public class Manager : IDisposable
    {
        bool disposed = false;

        // Key is the host's Id
        private readonly ConcurrentDictionary<int, HostState> _hostStates;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        
        public bool IsRunning { get; private set; }

        public Manager()
        {
            _hostStates = new ConcurrentDictionary<int, HostState>();
        }
        /// <summary>
        /// Ensure everything is set to run the BoincManager.
        /// -Load the data from database.
        /// </summary>
        /// <param name="hosts"></param>
        public void Initialize(IEnumerable<Host> hosts)
        {
            // Initialize the Dictionary. Add all the hosts stored in the database.
            foreach (var host in hosts)
            {
                AddHost(host);
            }
            // TODO - Connect in parallel
            // await Utils.ParallelForEachAsync(hosts, AddHost); // Connect to all the hosts in parallel, instead of sequential order
        }

        /// <summary>
        /// Start the BoincManager.
        /// Connect to all the host with the AutoConnect property turned on.
        /// Start a background process to get the latest updates from the Boinc Client in every X seconds.
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;

            await ConnectAll();

            await StartUpdateLoop();
        }
        
        private async Task StartUpdateLoop()
        {            
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Update();

                // The 'Delay' method should be at bottom, otherwise the 'Update' method would be called one more time unnecessarily, when cancellation is requested.
                await Task.Delay(2000);
            }
        }

        private async Task Update()
        {
            // TODO - Update in prallel
            // TODO - Update only the Viewed tabs (is that possible?)
            foreach (var hostState in _hostStates.Values)
            {
                if (hostState.Connected)
                {
                    await hostState.BoincState.UpdateProjects();
                    await hostState.BoincState.UpdateResults(); // Tasks
                    await hostState.BoincState.UpdateFileTransfers();
                    await hostState.BoincState.UpdateMessages();
                }
            }
        }

        /// <summary>
        /// Connect to a host.
        /// </summary>
        /// <param name="hostId">Host ID.</param>
        /// <returns></returns>
        public async Task Connect(int hostId)
        {
            var found = _hostStates.TryGetValue(hostId, out HostState hostState);
            if (!found)
                return;

            if (hostState.Connected || string.IsNullOrWhiteSpace(hostState.IpAddress) || string.IsNullOrEmpty(hostState.Password))
                return;

            hostState.Status = $"Connecting...";

            try
            {
                // Connecting to host
                await hostState.RpcClient.ConnectAsync(hostState.IpAddress, hostState.Port);
                hostState.Authorized = await hostState.RpcClient.AuthorizeAsync(hostState.Password);

                if (hostState.Authorized)
                {
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

        // TODO - Paralell
        public async Task ConnectAll(bool onlyHostsWithAutoConnect = true)
        {
            // Connect to all hosts where AutoConnect property is set to TRUE.
            if (onlyHostsWithAutoConnect)
            {
                foreach (var hostState in _hostStates.Values)
                {
                    if (hostState.AutoConnect)
                    {
                        await Connect(hostState.Id);
                    }
                }

            }
            else // Connect to all hosts
            {
                foreach (var hostId in _hostStates.Keys)
                {
                    await Connect(hostId);
                }
            }
        }

        /// <summary>
        /// Disconnect a host.
        /// </summary>
        /// <param name="hostId">Host ID.</param>
        public void Disconnect(int hostId)
        {
            var found = _hostStates.TryGetValue(hostId, out HostState hostState);
            if (found && hostState.Connected)
            {
                hostState.Status = string.Empty;
                hostState.Disconnect();
            }
        }
        
        // TODO - Paralell
        public void DisconnectAll()
        {
            foreach (var hostId in _hostStates.Keys)
            {
                Disconnect(hostId);
            }
        }
        
        /// <summary>
        /// Stop the BoincManager:
        /// Stop the background process which gets the update from the Boinc Client.
        /// Disconnect all the hosts.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            _cancellationTokenSource.Cancel();

            DisconnectAll();
        }

        public void AddHost(Host host)
        {
            _hostStates.TryAdd(host.Id, new HostState(host));
        }

        public void RemoveHost(int hostId)
        {
            var found = _hostStates.TryGetValue(hostId, out HostState hostState);
            if (found)
            {
                hostState.Dispose();
            }

            _hostStates.TryRemove(hostId, out _);
        }

        public void UpdateHost(Host host)
        {
            var found = _hostStates.TryGetValue(host.Id, out HostState hostState);
            if (found)
            {
                hostState.Update(host);
            }
        }

        public HostState GetHostState(int id)
        {
            _hostStates.TryGetValue(id, out HostState hostState);

            return hostState;
        }

        public IEnumerable<HostState> GetAllHostStates()
        {
            return _hostStates.Values;
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
                if (IsRunning)
                {
                    IsRunning = false;
                    _cancellationTokenSource.Cancel();
                }

                // TODO - Paralell
                foreach (var hostId in _hostStates.Keys)
                {
                    RemoveHost(hostId);
                }

                _cancellationTokenSource.Dispose();
            }

            disposed = true;
        }
    }
}
