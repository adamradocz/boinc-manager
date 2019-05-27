using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BoincManager.Models;
using BoincManagerWindows.Models;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class MainViewModel : BindableBase, IDisposable
    {
        bool disposed = false;

        private readonly BoincManager.Manager _manager;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public List<ComputerGorupViewModel> ComputerGroups { get; }
        public ObservableCollection<HostViewModel> Computers { get; }
        public ObservableCollection<ProjectViewModel> Projects { get; }
        public ObservableCollection<TaskViewModel> Tasks { get; }
        public ObservableCollection<TransferViewModel> Transfers { get; }
        public ObservableCollection<MessageViewModel> Messages { get; }

        string status;
        public string Status
        {
            get { return status; }
            private set { SetProperty(ref status, value); }
        }

        int currentTabPage;
        public int CurrentTabPage
        {
            get { return currentTabPage; }
            set { SetProperty(ref currentTabPage, value); }
        }

        public HostViewModel SelectedComputerInTreeView { get; set; }
        public IList SelectedComputers { get; set; }
        public IList SelectedProjects { get; set; }
        public IList SelectedTasks { get; set; }
        public IList SelectedTransfers { get; set; }
        public IList SelectedMessages { get; set; }

        // File menu
        public ICommand CloseCommand { get; }
        public ICommand CloseAndStopBoincCommand { get; }

        // Extras menu
        public ICommand RunBenchmarksCommand { get; }

        // Computers
        public ICommand AddComputerCommand { get; }
        public ICommand RemoveComputerCommand { get; }
        public ICommand ConnectComputerCommand { get; }
        public ICommand DisconnectComputerCommand { get; }

        // Project tab
        public ICommand AttachProjectCommand { get; }
        public ICommand UpdateProjectCommand { get; }
        public ICommand SuspendProjectCommand { get; }
        public ICommand NoNewTasksProjectCommand { get; }
        public ICommand ResetProjectCommand { get; }
        public ICommand DetachProjectCommand { get; }

        // Tasks tab
        public ICommand ShowGraphicsCommand { get; }
        public ICommand SuspendTaskCommand { get; }
        public ICommand AbortTaskCommand { get; }

        // Transfers tab
        public ICommand RetryTransferCommand { get; }
        public ICommand AbortTransferCommand { get; }

        // Messages tab
        public ICommand CopyMessagesCommand { get; }

        public MainViewModel()
        {
            _manager = new BoincManager.Manager();
            _cancellationTokenSource = new CancellationTokenSource();

            ComputerGroups = new List<ComputerGorupViewModel>();
            Computers = new ObservableCollection<HostViewModel>();
            Projects = new ObservableCollection<ProjectViewModel>();
            Tasks = new ObservableCollection<TaskViewModel>();
            Transfers = new ObservableCollection<TransferViewModel>();
            Messages = new ObservableCollection<MessageViewModel>();

            // File menu
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            CloseAndStopBoincCommand = new RelayCommand(ExecuteCloseAndStopBoincCommand, CanExecuteCloseAndStopBoincCommand);

            // Extras menu
            RunBenchmarksCommand = new RelayCommand(ExecuteRunBenchmarksCommand, IsComputerSelected);

            // Computers
            AddComputerCommand = new RelayCommand(ExecuteAddComputerCommand);
            RemoveComputerCommand = new RelayCommand(ExecuteRemoveComputerCommand, IsComputerSelected);
            ConnectComputerCommand = new RelayCommand(ExecuteConnectComputerCommand, CanExecuteConnectComputerCommand);
            DisconnectComputerCommand = new RelayCommand(ExecuteDisconnectComputerCommand, CanExecuteDisconnectComputerCommand);

            // Projects tab
            AttachProjectCommand = new RelayCommand(ExecuteAttachProjectCommand, IsComputerSelected);
            UpdateProjectCommand = new RelayCommand(ExecuteUpdateProjectCommand, IsProjectSelected);
            SuspendProjectCommand = new RelayCommand(ExecuteSuspendProjectCommand, IsProjectSelected);
            NoNewTasksProjectCommand = new RelayCommand(ExecuteNoNewTasksProjectCommand, IsProjectSelected);
            ResetProjectCommand = new RelayCommand(ExecuteResetProjectCommand, IsProjectSelected);
            DetachProjectCommand = new RelayCommand(ExecuteDetachProjectCommand, IsProjectSelected);

            // Tasks tab
            ShowGraphicsCommand = new RelayCommand(ExecuteShowGraphicsCommand, CanExecuteShowGraphicsCommand);
            SuspendTaskCommand = new RelayCommand(ExecuteSuspendTaskCommand, CanExecuteSuspendTaskCommand);
            AbortTaskCommand = new RelayCommand(ExecuteAbortTaskCommand, CanExecuteAbortTaskCommand);

            // Transfers tab
            RetryTransferCommand = new RelayCommand(ExecuteRetryTransferCommand, CanExecuteRetryTransferCommand);
            AbortTransferCommand = new RelayCommand(ExecuteAbortTransferCommand, CanExecuteAbortTransferCommand);

            // Messages tab
            CopyMessagesCommand = new RelayCommand(ExecuteCopyMessagesCommand, CanExecuteCopyMessagesCommand);

            // Creating groups for the Tree View
            var computerGroup = new ComputerGorupViewModel("All")
            {
                Members = Computers
            };
            ComputerGroups.Add(computerGroup);
        }

        private void ExecuteCloseCommand()
        {
            Close();

            Application.Current.Shutdown();
        }

        private async void ExecuteCloseAndStopBoincCommand()
        {
            if (MessageBox.Show($"Stop BOINC on localhost. Are you sure?", "Stop BOINC and close BOINC Manager", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            var hostStates = _manager.GetAllHostStates();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected && hostState.Localhost)
                {
                    await hostState.RpcClient.QuitAsync();
                    break;
                }
            }

            ExecuteCloseCommand();
        }

        private bool CanExecuteCloseAndStopBoincCommand()
        {
            var hostStates = _manager.GetAllHostStates();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected && hostState.Localhost)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteRunBenchmarksCommand()
        {
            foreach(HostViewModel selectedComputer in SelectedComputers)
            {
                await _manager.GetHostState(selectedComputer.Id).RpcClient.RunBenchmarksAsync();
            }
        }

        private bool IsComputerSelected()
        {
            return SelectedComputers != null && SelectedComputers.Count != 0;
        }

        private async void ExecuteAddComputerCommand()
        {
            var host = new Host("New host", "localhost", "123");
            using (var db = new ApplicationDbContext())
            {
                db.Host.Add(host);
                await db.SaveChangesAsync();
            }

            _manager.AddHost(host);
            Computers.Add(new HostViewModel(_manager.GetHostState(host.Id)));
        }
        
        private async void ExecuteRemoveComputerCommand()
        {
            string messageBoxText = SelectedComputers.Count == 1
                ? string.Format("Removing computer {0}. Are you sure?", ((HostViewModel)SelectedComputers[0]).Name)
                : string.Format("Removing {0} computers. Are you sure?", SelectedComputers.Count);

            if (MessageBox.Show(messageBoxText, "Removing computer(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            // Remove the selected computers from Model
            List<int> removableComputerIds = new List<int>();
            using (var db = new ApplicationDbContext())
            {
                foreach (HostViewModel computerVM in SelectedComputers)
                {
                    _manager.RemoveHost(computerVM.Id);

                    var removableHost = await db.Host.FindAsync(computerVM.Id);
                    if (removableHost != null)
                    {
                        db.Host.Remove(removableHost);
                    }
                    
                    removableComputerIds.Add(computerVM.Id);
                }

                await db.SaveChangesAsync();
            }

            // Remove the selected computers from ViewModel
            foreach (var removableComputerId in removableComputerIds)
            {
                for (int i = 0; i < Computers.Count; i++)
                {
                    if (removableComputerId == Computers[i].Id)
                    {
                        Computers.RemoveAt(i);
                        break;
                    }
                }
            }            
        }

        private async void ExecuteConnectComputerCommand()
        {
            foreach (HostViewModel computer in SelectedComputers)
            {
                if (!computer.Connected)
                {
                    await _manager.Connect(computer.Id);
                }
            }

            Update();
        }

        private bool CanExecuteConnectComputerCommand()
        {
            if (SelectedComputers != null && SelectedComputers.Count != 0)
            {
                foreach (HostViewModel computer in SelectedComputers)
                {
                    if (!computer.Connected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ExecuteDisconnectComputerCommand()
        {
            foreach (HostViewModel computer in SelectedComputers)
            {
                if (computer.Connected)
                {
                    _manager.Disconnect(computer.Id);
                }
            }

            Update();
        }

        private bool CanExecuteDisconnectComputerCommand()
        {
            if (SelectedComputers != null && SelectedComputers.Count != 0)
            {
                foreach (HostViewModel computer in SelectedComputers)
                {
                    if (computer.Connected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ExecuteAttachProjectCommand()
        {
            var updateNeeded = false;
            foreach (HostViewModel selectedComputer in SelectedComputers)
            {
                AttachToProjectWindow window = new AttachToProjectWindow(_manager.GetHostState(selectedComputer.Id).RpcClient)
                {
                    Owner = Application.Current.MainWindow
                };

                if (window.ShowDialog() == true)
                {
                    updateNeeded = true;
                }
            }           

            if (updateNeeded)
            {
                Update();
            }
        }

        private async void ExecuteUpdateProjectCommand()
        {
            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Update);
            }

            Update();
        }

        private bool IsProjectSelected()
        {
            return SelectedProjects != null && SelectedProjects.Count != 0;
        }

        private async void ExecuteSuspendProjectCommand()
        {
            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                if (!selectedProject.Project.Suspended)
                {
                    await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Suspend);
                }
            }

            Update();
        }

        private async void ExecuteNoNewTasksProjectCommand()
        {
            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                if (!selectedProject.Project.DontRequestMoreWork)
                {
                    await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.NoMoreWork);
                }
            }

            Update();
        }

        private async void ExecuteResetProjectCommand()
        {
            string messageBoxText = SelectedProjects.Count == 1
                ? $"Resetting project {((ProjectViewModel)SelectedProjects[0]).Name}. Are you sure?"
                : $"Resetting {SelectedProjects.Count} projects. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Resetting project(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Reset);
            }

            Update();
        }

        private async void ExecuteDetachProjectCommand()
        {
            string messageBoxText = SelectedProjects.Count == 1
                ? $"Detaching project {((ProjectViewModel)SelectedProjects[0]).Name}. Are you sure?"
                : $"Detaching {SelectedProjects.Count} projects. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Detaching project(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Detach);
            }

            Update();
        }        

        private void ExecuteShowGraphicsCommand()
        {
            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (_manager.GetHostState(selectedTask.HostId).Localhost && selectedTask.RpcResult.GraphicsAvailable)
                {
                    GraphicsAppLauncher.StartGraphicsAppOrBringToTop(selectedTask.RpcResult.GraphicsExecPath, selectedTask.RpcResult.SlotPath);
                }
            }
        }

        private bool CanExecuteShowGraphicsCommand()
        {
            if (SelectedTasks == null || SelectedTasks.Count == 0)
                return false;

            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (_manager.GetHostState(selectedTask.HostId).Localhost && selectedTask.RpcResult.GraphicsAvailable)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteSuspendTaskCommand()
        {
            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (selectedTask.RpcResult.Suspendable && !selectedTask.RpcResult.Suspended)
                {
                    await _manager.GetHostState(selectedTask.HostId).RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, ResultOperation.Suspend);
                }
                else if (selectedTask.RpcResult.Suspended)
                {
                    await _manager.GetHostState(selectedTask.HostId).RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, ResultOperation.Resume);
                }
            }

            Update();
        }

        private bool CanExecuteSuspendTaskCommand()
        {
            if (SelectedTasks == null || SelectedTasks.Count == 0)
                return false;

            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (selectedTask.RpcResult.Suspendable)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteAbortTaskCommand()
        {
            string messageBoxText = SelectedTasks.Count == 1
                ? $"Aborting task {((TaskViewModel)SelectedTasks[0]).Workunit}. Are you sure?"
                : $"Aborting {SelectedTasks.Count} tasks. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Aborting task(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
            
            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (selectedTask.RpcResult.Abortable)
                {
                    await _manager.GetHostState(selectedTask.HostId).RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, ResultOperation.Abort);
                }
            }
            
            Update();
        }

        private bool CanExecuteAbortTaskCommand()
        {
            if (SelectedTasks == null || SelectedTasks.Count == 0)
                return false;

            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (selectedTask.RpcResult.Abortable)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteRetryTransferCommand()
        {
            foreach (TransferViewModel selectedTransfer in SelectedTransfers)
            {
                await _manager.GetHostState(selectedTransfer.HostId).RpcClient.PerformFileTransferOperationAsync(selectedTransfer.FileTransfer, FileTransferOperation.Retry);
            }

            Update();
        }

        private bool CanExecuteRetryTransferCommand()
        {
            return SelectedTransfers != null && SelectedTransfers.Count != 0;
        }

        private async void ExecuteAbortTransferCommand()
        {
            string messageBoxText = SelectedTransfers.Count == 1
                ? $"Aborting transfer {((TransferViewModel)SelectedTransfers[0]).FileName}. Are you sure?"
                : $"Aborting {SelectedTransfers.Count} transfer. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Aborting transfer(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (TransferViewModel selectedTransfer in SelectedTransfers)
            {
                await _manager.GetHostState(selectedTransfer.HostId).RpcClient.PerformFileTransferOperationAsync(selectedTransfer.FileTransfer, FileTransferOperation.Abort);
            }

            Update();
        }

        private bool CanExecuteAbortTransferCommand()
        {
            return SelectedTransfers != null && SelectedTransfers.Count != 0;
        }

        private void ExecuteCopyMessagesCommand()
        {
            var text = new StringBuilder();
            foreach (MessageViewModel selectedMessage in SelectedMessages)
            {
                text.AppendLine(selectedMessage.Message);
            }

            Clipboard.SetText(text.ToString());
        }

        private bool CanExecuteCopyMessagesCommand()
        {
            return SelectedMessages != null && SelectedMessages.Count != 0;
        }

        public async Task StartBoincManager()
        {
            // Get host data from database.
            status = "Loading database...";
            List<Host> hosts;
            using (var db = new ApplicationDbContext())
            {
                hosts = db.Host.ToList();
            }

            // Start the Boinc Manager
            status = "Connecting...";
            _manager.Initialize(hosts);
            _manager.Start();

            // Initialize the View
            status = "Updating...";
            var hostStates = _manager.GetAllHostStates();
            foreach (var hostState in hostStates)
            {
                Computers.Add(new HostViewModel(hostState)); // Hosts tab
                UpdateProject(hostState);                    // Projects tab                
                UpdateTask(hostState);                       // Tasks tab
                UpdateTransfer(hostState);                   // Transfers tab
                UpdateMessages(hostState);                   // Messages tab
            }

            _cancellationToken = _cancellationTokenSource.Token;
            await StartUpdateLoop(_cancellationToken);
        }

        private async Task StartUpdateLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Update();

                // The 'Delay' method should be at bottom, otherwise the 'Update' method would be called one mroe time unnecessarily, when cancellation is requested.
                await Task.Delay(2000);
            }
        }

        /// <summary>
        /// Update the Models and the ViewModels, but only on the visible tabs.
        /// </summary>
        /// <returns></returns>
        public void Update()
        {
            Status = "Updating...";

            var filteredHosts = GetFilteredHosts();
            // TODO - The Parallel update is not working because something is not thread-safe
            //await BoincManager.Utils.ParallelForEachAsync(filteredHosts.Values, GetNewBoincInfo); // Update in parallel
            foreach (var hostState in filteredHosts)
            {
                GetNewBoincInfo(hostState);
            }
            
            // Remove outdated info
            switch (CurrentTabPage)
            {
                case 1: // Projects tab
                    RemoveOutdatedProject(filteredHosts);
                    break;
                case 2: // Tasks tab
                    RemoveOutdatedTask(filteredHosts);
                    break;
                case 3: // Transfers tab
                    RemoveOutdatedTransfer(filteredHosts);
                    break;
                case 5: // Messages tab
                    RemoveOutdatedMessage(filteredHosts);
                    break;
            }
            
            Status = string.Empty;
        }

        private void GetNewBoincInfo(HostState hostState)
        {
            try
            {
                switch (CurrentTabPage)
                {
                    case 0: // Hosts tab
                        UpdateHosts(hostState);
                        break;
                    case 1: // Projects tab                        
                        UpdateProject(hostState);
                        break;
                    case 2: // Tasks tab                        
                        UpdateTask(hostState);
                        break;
                    case 3: // Transfers tab                        
                        UpdateTransfer(hostState);
                        break;
                    case 5: // Messages tab
                        UpdateMessages(hostState);
                        break;
                }
            }
            catch(Exception e)
            {
                var computer = Computers.FirstOrDefault(c => c.Id == hostState.Id);
                if(computer != null)
                    computer.Status = $"Error: {e.Message}";
            }
        }

        private IEnumerable<HostState> GetFilteredHosts()
        {
            if (SelectedComputerInTreeView == null) // No computer selected
            {
                return _manager.GetAllHostStates(); // All the HostsStates
            }
            else if (!_manager.GetHostState(SelectedComputerInTreeView.Id).Connected) // Computer is not connected
            {
                return new HostState[0];
            }
            else // The selected computer
            {                
                return new List<HostState>
                {
                     _manager.GetHostState(SelectedComputerInTreeView.Id)
                };
            }
        }

        private void UpdateHosts(HostState hostState)
        {
            HostViewModel hostViewModel = Computers.FirstOrDefault(hVm => hVm.Id == hostState.Id);
            hostViewModel.Update(hostState);
        }

        private void UpdateProject(HostState hostState)
        {
            if (hostState.Connected)
            {
                foreach (Project project in hostState.BoincState.Projects)
                {
                    ProjectViewModel projectViewModel = Projects.FirstOrDefault(pvm => pvm.HostId == hostState.Id && pvm.Name == project.ProjectName);

                    if (projectViewModel == null)
                    {
                        projectViewModel = new ProjectViewModel(hostState);
                        projectViewModel.Update(project);
                        Projects.Add(projectViewModel);
                    }
                    else
                    {
                        projectViewModel.Update(project);
                    }
                }
            }
        }

        private void RemoveOutdatedProject(IEnumerable<HostState> hostStates)
        {
            var allProjects = new List<Project>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    allProjects.AddRange(hostState.BoincState.Projects);
                }
            }

            for (int i = 0; i < Projects.Count; i++)
            {
                if (!allProjects.Contains(Projects[i].Project))
                {
                    Projects.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateTask(HostState hostState)
        {
            if (hostState.Connected)
            {
                foreach (Result result in hostState.BoincState.Results)
                {
                    TaskViewModel taskViewModel = Tasks.FirstOrDefault(tvm => tvm.HostId == hostState.Id && tvm.Workunit == result.WorkunitName);

                    if (taskViewModel == null)
                    {
                        taskViewModel = new TaskViewModel(hostState);
                        taskViewModel.Update(result, hostState);
                        Tasks.Add(taskViewModel);
                    }
                    else
                    {
                        taskViewModel.Update(result, hostState);
                    }
                }
            }            
        }

        private void RemoveOutdatedTask(IEnumerable<HostState> hostStates)
        {
            var allTasks = new List<Result>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    allTasks.AddRange(hostState.BoincState.Results);
                }                
            }

            for (int i = 0; i < Tasks.Count; i++)
            {
                if (!allTasks.Contains(Tasks[i].RpcResult))
                {
                    Tasks.RemoveAt(i);
                    i--;
                }
            }
        }
        
        private void UpdateTransfer(HostState hostState)
        {
            if (hostState.Connected)
            {
                foreach (FileTransfer fileTransfer in hostState.BoincState.FileTransfers)
                {
                    TransferViewModel transferVM = Transfers.FirstOrDefault(tvm => tvm.HostId == hostState.Id && tvm.FileName == fileTransfer.Name);

                    if (transferVM == null)
                    {
                        transferVM = new TransferViewModel(hostState);
                        transferVM.Update(fileTransfer);
                        Transfers.Add(transferVM);
                    }
                    else
                    {
                        transferVM.Update(fileTransfer);
                    }
                }
            }            
        }

        private void RemoveOutdatedTransfer(IEnumerable<HostState> hostStates)
        {
            var allFileTransfers = new List<FileTransfer>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    allFileTransfers.AddRange(hostState.BoincState.FileTransfers);
                }                
            }

            for (int i = 0; i < Transfers.Count; i++)
            {
                if (!allFileTransfers.Contains(Transfers[i].FileTransfer))
                {
                    Transfers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void UpdateMessages(HostState hostState)
        {
            if (hostState.Connected)
            {
                foreach (Message message in hostState.BoincState.Messages)
                {
                    MessageViewModel messageVM = Messages.FirstOrDefault(mvm => mvm.HostId == hostState.Id);

                    if (messageVM == null)
                    {
                        messageVM = new MessageViewModel(hostState, message);
                        Messages.Add(messageVM);
                    }
                }
            }            
        }

        private void RemoveOutdatedMessage(IEnumerable<HostState> hostStates)
        {
            var allMessages = new List<Message>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    allMessages.AddRange(hostState.BoincState.Messages);
                }                
            }

            for (int i = 0; i < Messages.Count; i++)
            {
                if (!allMessages.Contains(Messages[i].RpcMessage))
                {
                    Messages.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Close()
        {
            _cancellationTokenSource.Cancel();
            _manager.Close();
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
                _cancellationTokenSource.Dispose();
                _manager.Dispose();
            }

            disposed = true;
        }

    }
}
