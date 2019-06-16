using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using BoincManager.Interfaces;
using BoincManager.Models;

namespace BoincManager
{
    public class Manager : IDisposable
    {
        bool disposed = false;

        // Key is the host's Id
        private readonly ConcurrentDictionary<int, HostState> _hostStates;

        private CancellationTokenSource _updateLoopCancellationTokenSource;
        private CancellationToken _updateLoopCancellationToken;

        private CancellationTokenSource _delayedStopCancellationTokenSource;
        private CancellationToken _delayedStopCancellationToken;

        public bool IsRunning { get; private set; }
        public bool DelayedStopStarted { get; private set; }

        public Manager()
        {
            _hostStates = new ConcurrentDictionary<int, HostState>();
        }

        /// <summary>
        /// Ensure everything is set to run the BoincManager.
        /// -Load the data from database.
        /// </summary>
        /// <param name="hosts"></param>
        public void Initialize(IEnumerable<HostConnection> hosts)
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
            
            _updateLoopCancellationTokenSource = new CancellationTokenSource();
            _updateLoopCancellationToken = _updateLoopCancellationTokenSource.Token;

            await ConnectAll();

            await StartUpdateLoop();
        }

        private async Task StartUpdateLoop()
        {            
            while (!_updateLoopCancellationToken.IsCancellationRequested)
            {
                await Update();

                // The 'Delay' method have to be at bottom, otherwise the 'Update' method would be called one more time unnecessarily, when cancellation is requested.
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
                hostState.InitializeConnection();
                await hostState.RpcClient.ConnectAsync(hostState.IpAddress, hostState.Port);
                hostState.Authorized = await hostState.RpcClient.AuthorizeAsync(hostState.Password);

                if (hostState.Authorized)
                {
                    hostState.Status = "Connected. Updating...";

                    hostState.Connected = true;

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
                hostState.Connected = false;                
                hostState.TerminateConnection();
                hostState.Status = string.Empty;
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

            _updateLoopCancellationTokenSource.Cancel();
            DisconnectAll();

            IsRunning = false;
        }

        /// <summary>
        /// Stop the BoincManager after x milliseconds.
        /// </summary>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        public async Task Stop(int millisecondsDelay)
        {
            if (DelayedStopStarted)
                return;

            DelayedStopStarted = true;

            _delayedStopCancellationTokenSource = new CancellationTokenSource();
            _delayedStopCancellationToken = _delayedStopCancellationTokenSource.Token;

            await Task.Delay(millisecondsDelay);

            if (!_delayedStopCancellationToken.IsCancellationRequested)
            {
                Stop();
                DelayedStopStarted = false;
            }
        }

        /// <summary>
        /// Terminate the delayed Stop()
        /// </summary>
        public void TerminateDelayedStop()
        {
            if (DelayedStopStarted)
            {
                _delayedStopCancellationTokenSource.Cancel();
                DelayedStopStarted = false;
            }
        }

        public void AddHost(HostConnection host)
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

        public void UpdateHost(HostConnection host)
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
                if (_delayedStopCancellationTokenSource != null)
                {
                    TerminateDelayedStop();
                    _delayedStopCancellationTokenSource.Dispose();
                }                

                // Stop the update loop
                if (IsRunning)
                {
                    IsRunning = false;
                    _updateLoopCancellationTokenSource.Cancel();
                }
                _updateLoopCancellationTokenSource.Dispose();

                // TODO - Paralell
                foreach (var hostId in _hostStates.Keys)
                {
                    RemoveHost(hostId);
                }
            }

            disposed = true;
        }
    }
}
