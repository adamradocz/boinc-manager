using BoincManager.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BoincManager
{
    public class Manager : IDisposable
    {
        bool disposed = false;

        // Key is the host's Id
        private readonly ConcurrentDictionary<int, HostState> _hostStates;

        private bool _useObservableCollections;

        private CancellationTokenSource _updateLoopCancellationTokenSource;
        private CancellationToken _updateLoopCancellationToken;

        private CancellationTokenSource _delayedStopCancellationTokenSource;
        private CancellationToken _delayedStopCancellationToken;

        private bool _updating;

        public bool IsRunning { get; private set; }

        /// <summary>
        /// Set X milliseconds Delay in the update loop in the <see cref="Start"/>.
        /// </summary>
        public int UpdatePeriod { get; set; } = 2000;
        public bool DelayedStopStarted { get; private set; }

        public string SearchString { get; set; }

        // The ObservableCollections should be ObservableDictionarys. That'd simplify a lot of code related to Update.
        public ObservableCollection<ObservableHost> Hosts { get; private set; }
        public ObservableCollection<ObservableProject> Projects { get; private set; }
        public ObservableCollection<ObservableTask> Tasks { get; private set; }
        public ObservableCollection<ObservableTransfer> Transfers { get; private set; }
        public ObservableCollection<ObservableMessage> Messages { get; private set; }

        public Manager()
        {
            _hostStates = new ConcurrentDictionary<int, HostState>();
        }

        /// <summary>
        /// Ensure everything is set to run the BoincManager.
        /// -Load the data from database.
        /// </summary>
        /// <param name="hosts"></param>
        /// <param name="useObservableCollections">Initialize the Observable Collections which can be used in MVVM framework.</param>
        public void Initialize(IEnumerable<HostConnection> hosts, bool useObservableCollections)
        {
            // Initialize the Observable Collections
            _useObservableCollections = useObservableCollections;
            if (_useObservableCollections)
            {
                Hosts = new ObservableCollection<ObservableHost>();
                Projects = new ObservableCollection<ObservableProject>();
                Tasks = new ObservableCollection<ObservableTask>();
                Transfers = new ObservableCollection<ObservableTransfer>();
                Messages = new ObservableCollection<ObservableMessage>();
            }

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

            // Update loop
            while (!_updateLoopCancellationToken.IsCancellationRequested)
            {
                await Update();
                await Task.Delay(UpdatePeriod);
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

        #region Update section
        public async Task Update()
        {
            if (_updating)
                return;

            _updating = true;

            // TODO - Update in prallel
            // TODO - Update only the Viewed tabs (is that possible?)
            foreach (var hostState in _hostStates.Values)
            {
                if (_updateLoopCancellationToken.IsCancellationRequested)
                    break;

                if (hostState.Connected)
                {
                    await UpdateProjects(hostState);
                    await UpdateTasks(hostState);
                    await UpdateTransfers(hostState);
                    await UpdateMessages(hostState);
                }
            }

            if (_useObservableCollections)
            {
                RemoveOutdatedProjects(_hostStates.Values);
                RemoveOutdatedTasks(_hostStates.Values);
                RemoveOutdatedTransfers(_hostStates.Values);
                RemoveOutdatedMessages(_hostStates.Values);
            }

            _updating = false;
        }

        private async Task UpdateProjects(HostState hostState)
        {
            await hostState.BoincState.UpdateProjects();

            if (!_useObservableCollections)
                return;

            foreach (var rpcProject in hostState.BoincState.Projects)
            {
                ObservableProject project = Projects.FirstOrDefault(pvm => pvm.HostId == hostState.Id && pvm.Name == rpcProject.ProjectName);
                if (project == null)
                {
                    project = new ObservableProject(hostState, rpcProject);
                    if (string.IsNullOrEmpty(SearchString))
                    {
                        Projects.Add(project);
                    }
                    else
                    {
                        foreach (var content in project.GetContentsForFiltering())
                        {
                            if (content != null && content.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                // The search string is found in any of the VM's property
                                Projects.Add(project);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    project.Update(rpcProject);
                }
            }
        }

        private void RemoveOutdatedProjects(IEnumerable<HostState> hostStates)
        {
            var currentProjects = new HashSet<BoincRpc.Project>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    currentProjects.UnionWith(hostState.BoincState.Projects);
                }
            }

            for (int i = 0; i < Projects.Count; i++)
            {
                if (!currentProjects.Contains(Projects[i].RpcProject))
                {
                    Projects.RemoveAt(i);
                    i--;
                }
            }
        }

        private async Task UpdateTasks(HostState hostState)
        {
            await hostState.BoincState.UpdateResults();

            if (!_useObservableCollections)
                return;

            foreach (var rpcResult in hostState.BoincState.Results)
            {
                ObservableTask task = Tasks.FirstOrDefault(m => m.HostId == hostState.Id && m.Workunit == rpcResult.WorkunitName);
                if (task == null)
                {
                    task = new ObservableTask(hostState, rpcResult);
                    if (string.IsNullOrEmpty(SearchString))
                    {
                        Tasks.Add(task);
                    }
                    else
                    {
                        foreach (var content in task.GetContentsForFiltering())
                        {
                            if (content != null && content.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                // The search string is found in any of the Models's property
                                Tasks.Add(task);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    task.Update(hostState, rpcResult);
                }
            }
        }

        private void RemoveOutdatedTasks(IEnumerable<HostState> hostStates)
        {
            var currentTasks = new HashSet<BoincRpc.Result>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    currentTasks.UnionWith(hostState.BoincState.Results);
                }
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                if (!currentTasks.Contains(Tasks[i].RpcResult))
                {
                    Tasks.RemoveAt(i);
                    i--;
                }
                else if (!string.IsNullOrEmpty(SearchString))
                {
                    var searchStringFound = false;
                    foreach (var content in Tasks[i].GetContentsForFiltering())
                    {
                        if (content != null && content.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            // The search string is found in any of the Models's property
                            searchStringFound = true;                            
                            break;
                        }
                    }

                    if (!searchStringFound)
                    {
                        Tasks.RemoveAt(i);
                        i--;
                    }
                }
            }
         }

        private async Task UpdateTransfers(HostState hostState)
        {
            await hostState.BoincState.UpdateFileTransfers();

            if (!_useObservableCollections)
                return;

            foreach (BoincRpc.FileTransfer fileTransfer in hostState.BoincState.FileTransfers)
            {
                ObservableTransfer transfer = Transfers.FirstOrDefault(tvm => tvm.HostId == hostState.Id && tvm.FileName == fileTransfer.Name);
                if (transfer == null)
                {
                    transfer = new ObservableTransfer(hostState, fileTransfer);
                    if (string.IsNullOrEmpty(SearchString))
                    {
                        Transfers.Add(transfer);
                    }
                    else
                    {
                        foreach (var content in transfer.GetContentsForFiltering())
                        {
                            if (content != null && content.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                // The search string is found in any of the Models's property
                                Transfers.Add(transfer);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    transfer.Update(fileTransfer);
                }
            }
        }

        private void RemoveOutdatedTransfers(IEnumerable<HostState> hostStates)
        {
            var currentFileTransfers = new HashSet<BoincRpc.FileTransfer>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    currentFileTransfers.UnionWith(hostState.BoincState.FileTransfers);
                }
            }

            for (int i = 0; i < Transfers.Count; i++)
            {
                if (!currentFileTransfers.Contains(Transfers[i].FileTransfer))
                {
                    Transfers.RemoveAt(i);
                    i--;
                }
            }
        }

        private async Task UpdateMessages(HostState hostState)
        {
            await hostState.BoincState.UpdateMessages();

            if (!_useObservableCollections)
                return;

            foreach (BoincRpc.Message rpcMessage in hostState.BoincState.Messages)
            {
                ObservableMessage message = Messages.FirstOrDefault(mvm => mvm.HostId == hostState.Id);
                if (message == null)
                {
                    message = new ObservableMessage(hostState, rpcMessage);
                    if (string.IsNullOrEmpty(SearchString))
                    {
                        Messages.Add(message);
                    }
                    else
                    {
                        foreach (var content in message.GetContentsForFiltering())
                        {
                            if (content != null && content.IndexOf(SearchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                            {
                                // The search string is found in any of the Models's property
                                Messages.Add(message);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void RemoveOutdatedMessages(IEnumerable<HostState> hostStates)
        {
            var currentMessages = new HashSet<BoincRpc.Message>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    currentMessages.UnionWith(hostState.BoincState.Messages);
                }
            }

            for (int i = 0; i < Messages.Count; i++)
            {
                if (!currentMessages.Contains(Messages[i].RpcMessage))
                {
                    Messages.RemoveAt(i);
                    i--;
                }
            }
        }

        #endregion

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

            // This part of the code is not concurrent, but only ASP.NET uses concurrent calls, MVVM projects are not.
            // ASP.NET don't use ObservableCollections
            if (_useObservableCollections)
            {
                Hosts.Add(new ObservableHost(GetHostState(host.Id)));
            }
        }

        public void RemoveHost(int hostId)
        {
            var found = _hostStates.TryGetValue(hostId, out HostState hostState);
            if (found)
            {
                hostState.Dispose();
            }
            _hostStates.TryRemove(hostId, out _);

            // This part of the code is not concurrent, but only ASP.NET uses concurrent calls, MVVM projects are not.
            // ASP.NET don't use ObservableCollections
            if (_useObservableCollections)
            {
                for (int i = 0; i < Hosts.Count; i++)
                {
                    if (Hosts[i].Id == hostId)
                    {
                        Hosts.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void UpdateHost(HostConnection host)
        {
            var found = _hostStates.TryGetValue(host.Id, out HostState hostState);
            if (found)
            {
                hostState.Update(host);
            }

            // This part of the code is not concurrent, but only ASP.NET uses concurrent calls, MVVM projects are not.
            // ASP.NET don't use ObservableCollections
            if (_useObservableCollections)
            {
                for (int i = 0; i < Hosts.Count; i++)
                {
                    if (Hosts[i].Id == host.Id)
                    {
                        Hosts[i].Update(hostState);
                        break;
                    }
                }
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
