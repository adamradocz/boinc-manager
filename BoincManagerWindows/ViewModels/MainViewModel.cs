using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private static readonly object lockObject = new object(); // lockObject for EnableCollectionSynchronization method. Info: http://www.jonathanantoine.com/2011/09/24/wpf-4-5-part-7-accessing-collections-on-non-ui-threads/
        
        private readonly Dictionary<string, HostState> hostsState = new Dictionary<string, HostState>(); // Key is the Id of the host/computer

        public List<ComputerGorupViewModel> ComputerGroups { get; }
        public ObservableCollection<ComputerViewModel> Computers { get; }
        public ObservableCollection<ProjectViewModel> Projects { get; }
        public ObservableCollection<TaskViewModel> Tasks { get; }
        public ObservableCollection<TransferViewModel> Transfers { get; }
        public ObservableCollection<MessageViewModel> Messages { get; }

        string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        int currentTabPage;
        public int CurrentTabPage
        {
            get { return currentTabPage; }
            set { SetProperty(ref currentTabPage, value); }
        }

        public ComputerViewModel SelectedComputerInTreeView { get; set; }
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
            ComputerGroups = new List<ComputerGorupViewModel>();
            Computers = new ObservableCollection<ComputerViewModel>();
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
            foreach (var hostState in hostsState.Values)
            {
                hostState.Close();
                hostState.Dispose();
            }

            SaveConnectionData();

            Application.Current.Shutdown();
        }

        private void SaveConnectionData()
        {
            throw new NotImplementedException();
        }

        private async void ExecuteCloseAndStopBoincCommand()
        {
            if (MessageBox.Show($"Stop BOINC on localhost. Are you sure?", "Stop BOINC and close BOINC Manager", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (var hoststate in hostsState.Values)
            {
                if (hoststate.Localhost)
                {
                    await hoststate.RpcClient.QuitAsync();
                    break;
                }
            }

            ExecuteCloseCommand();
        }

        private bool CanExecuteCloseAndStopBoincCommand()
        {
            foreach (var hoststate in hostsState.Values)
            {
                if (hoststate.Localhost)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteRunBenchmarksCommand()
        {
            foreach(ComputerViewModel selectedComputer in SelectedComputers)
            {
                await hostsState[selectedComputer.Id].RpcClient.RunBenchmarksAsync();
            }
        }

        private bool IsComputerSelected()
        {
            return SelectedComputers != null && SelectedComputers.Count != 0;
        }

        private void ExecuteAddComputerCommand()
        {
            var id = Guid.NewGuid().ToString();
            Computers.Add(new ComputerViewModel(id, "New Computer", string.Empty, BoincManager.Constants.BoincDefaultPort, string.Empty));
        }

        private void ExecuteRemoveComputerCommand()
        {
            string messageBoxText = SelectedComputers.Count == 1
                ? string.Format("Removing computer {0}. Are you sure?", ((ComputerViewModel)SelectedComputers[0]).Name)
                : string.Format("Removing {0} computers. Are you sure?", SelectedComputers.Count);

            if (MessageBox.Show(messageBoxText, "Removing computer(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            // Remove the selected computers from Model
            List<string> removableComputerIds = new List<string>();
            foreach (ComputerViewModel computerVM in SelectedComputers)
            {
                removableComputerIds.Add(computerVM.Id);

                if (hostsState.ContainsKey(computerVM.Id))
                {
                    hostsState[computerVM.Id].Close();
                    hostsState[computerVM.Id].Dispose();
                    hostsState.Remove(computerVM.Id);
                }
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
            foreach (ComputerViewModel computer in SelectedComputers)
            {
                if (!computer.Connected)
                {
                    await Connect(computer);
                }
            }
        }

        private bool CanExecuteConnectComputerCommand()
        {
            if (SelectedComputers != null && SelectedComputers.Count != 0)
            {
                foreach (ComputerViewModel computer in SelectedComputers)
                {
                    if (!computer.Connected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async void ExecuteAttachProjectCommand()
        {
            var updateNeeded = false;
            foreach (ComputerViewModel selectedComputer in SelectedComputers)
            {
                AttachToProjectWindow window = new AttachToProjectWindow(hostsState[selectedComputer.Id].RpcClient)
                {
                    Owner = Application.Current.MainWindow
                };

                if (window.ShowDialog() == true)
                    updateNeeded = true;
            }           

            if (updateNeeded)
            {
                await Update();
            }
        }

        private async void ExecuteUpdateProjectCommand()
        {
            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                await hostsState[selectedProject.ComputerId].RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Update);
            }

            await Update();
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
                    await hostsState[selectedProject.ComputerId].RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Suspend);
                }
            }

            await Update();
        }

        private async void ExecuteNoNewTasksProjectCommand()
        {
            foreach (ProjectViewModel selectedProject in SelectedProjects)
            {
                if (!selectedProject.Project.DontRequestMoreWork)
                {
                    await hostsState[selectedProject.ComputerId].RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.NoMoreWork);
                }
            }

            await Update();
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
                await hostsState[selectedProject.ComputerId].RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Reset);
            }

            await Update();
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
                await hostsState[selectedProject.ComputerId].RpcClient.PerformProjectOperationAsync(selectedProject.Project, ProjectOperation.Detach);
            }

            await Update();
        }        

        private void ExecuteShowGraphicsCommand()
        {
            foreach (TaskViewModel selectedTask in SelectedTasks)
            {
                if (hostsState[selectedTask.ComputerId].Localhost && selectedTask.RpcResult.GraphicsAvailable)
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
                if (hostsState[selectedTask.ComputerId].Localhost && selectedTask.RpcResult.GraphicsAvailable)
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
                    await hostsState[selectedTask.ComputerId].RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, ResultOperation.Suspend);
                }
                else if (selectedTask.RpcResult.Suspended)
                {
                    await hostsState[selectedTask.ComputerId].RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, ResultOperation.Resume);
                }
            }

            await Update();
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
                    await hostsState[selectedTask.ComputerId].RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, ResultOperation.Abort);
                }
            }
            
            await Update();
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
                await hostsState[selectedTransfer.ComputerId].RpcClient.PerformFileTransferOperationAsync(selectedTransfer.FileTransfer, FileTransferOperation.Retry);
            }

            await Update();
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
                await hostsState[selectedTransfer.ComputerId].RpcClient.PerformFileTransferOperationAsync(selectedTransfer.FileTransfer, FileTransferOperation.Abort);
            }

            await Update();
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
        
        public async Task ConnectToAllComputers()
        {
            status = "Connecting...";

            // Connect to all the computers
            await BoincManager.Utils.ParallelForEachAsync(Computers, Connect);
            //foreach (var computer in Computers) await Connect(computer);

#pragma warning disable CS4014
            BoincInfoUpdateLoop(CancellationToken.None);
#pragma warning restore CS4014
        }
        
        public async Task Connect(ComputerViewModel computer)
        {
            if (string.IsNullOrWhiteSpace(computer.IpAddress) || string.IsNullOrEmpty(computer.Password) || computer.Connected)
            {
                return;
            }

            computer.Status = $"Connecting...";

            try
            {
                HostState hostState = new HostState(computer.Id, computer.Name);

                // Connecting to host
                await hostState.RpcClient.ConnectAsync(computer.IpAddress, computer.Port);
                hostState.Authorized = await hostState.RpcClient.AuthorizeAsync(computer.Password);

                if (hostState.Authorized)
                {                    
                    // Since the ObservableCollections is created on UI thread, it can only be modified from UI thread and not from other threads.
                    await Application.Current.Dispatcher.Invoke(async delegate
                    {
                        computer.Status = "Connected. Updating...";

                        hostsState.Add(computer.Id, hostState);

                        // Updating the hostState Model
                        await hostState.BoincState.UpdateAll();

                        // Updating the ComputerViewModel
                        await computer.FirstUpdateOnConnect(hostState);

                        
                        UpdateProjectViewModels(hostState);
                        UpdateTaskViewModels(hostState);
                        UpdateTransferViewModels(hostState);
                        await UpdateMessages(hostState);
                    },
                    System.Windows.Threading.DispatcherPriority.Normal);
                    
                }
                else
                {
                    computer.Status = "Authorization error.";
                }

            }
            catch (Exception e)
            {
                computer.Status = $"Error connecting. {e.Message}";
            }
        }
        
        private async Task BoincInfoUpdateLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(2000, cancellationToken);
                await Update();
            }
        }

        /// <summary>
        /// Update the Models and the ViewModels, but only on the visible tabs.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            Status = "Updating...";

            // Enable synchronization with a collection that changes from different threads. The collection has to be syncronized because of the ParallelForEachAsync.
            // (Exception will be thrown when an item's source has changed from another thread and DataGrid doesn't receive a notification (CollectionChanged event) about ItemsSource being changed.)
            switch (CurrentTabPage)
            {             
                case 1: // Projects tab
                    BindingOperations.EnableCollectionSynchronization(Projects, lockObject);
                    break;
                case 2: // Tasks tab
                    BindingOperations.EnableCollectionSynchronization(Tasks, lockObject);
                    break;
                case 3: // Transfers tab
                    BindingOperations.EnableCollectionSynchronization(Transfers, lockObject);
                    break;
                case 5: // Messages tab
                    BindingOperations.EnableCollectionSynchronization(Messages, lockObject);
                    break;
            }

            var filteredHosts = GetFilteredHosts();
            await BoincManager.Utils.ParallelForEachAsync(filteredHosts.Values, UpdateParallel); // Update in parallel
            //foreach (var hostState in filteredHosts.Values) await UpdateParallel(hostState);
            
            switch (CurrentTabPage)
            {                
                case 1: // Projects tab
                    RemoveOutdatedProjectViewModels(filteredHosts);
                    break;
                case 2: // Tasks tab
                    RemoveOutdatedTaskViewModels(filteredHosts);
                    break;
                case 3: // Transfers tab
                    RemoveOutdatedTransferViewModels(filteredHosts);
                    break;
                case 5: // Messages tab
                    break;
            }
            
            Status = string.Empty;
        }

        private async Task UpdateParallel(HostState hostState)
        {
            try
            {
                switch (CurrentTabPage)
                {
                    case 1: // Projects tab
                        await UpdateProjects(hostState);
                        UpdateProjectViewModels(hostState);
                        break;
                    case 2: // Tasks tab
                        await UpdateTasks(hostState);
                        UpdateTaskViewModels(hostState);
                        break;
                    case 3: // Transfers tab
                        await UpdateTransfers(hostState);
                        UpdateTransferViewModels(hostState);
                        break;
                    case 5: // Messages tab
                        await UpdateMessages(hostState);
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

        private Dictionary<string, HostState> GetFilteredHosts()
        {            
            if (SelectedComputerInTreeView == null) // No computer selected
            {
                return hostsState; // All the list
            }
            else if (!hostsState.ContainsKey(SelectedComputerInTreeView.Id)) // Computer not connected, therefore no associated HostState available
            {
                return new Dictionary<string, HostState>();
            }
            else // The selected computer
            {                
                return new Dictionary<string, HostState>
                {
                    { SelectedComputerInTreeView.Id, hostsState[SelectedComputerInTreeView.Id] }
                };
            }
        }

        private async Task UpdateProjects(HostState hostState)
        {
            await hostState.BoincState.UpdateProjects();
        }

        private void UpdateProjectViewModels(HostState hostState)
        {
            foreach (Project project in hostState.BoincState.Projects)
            {
                ProjectViewModel projectViewModel = Projects.FirstOrDefault(pvm => pvm.ComputerId == hostState.Id && pvm.Name == project.ProjectName);
                
                if (projectViewModel == null)
                {
                    projectViewModel = new ProjectViewModel(hostState.Id, hostState.HostName);
                    projectViewModel.Update(project);
                    Projects.Add(projectViewModel);
                }
                else
                {
                    projectViewModel.Update(project);
                }
            }
        }

        private void RemoveOutdatedProjectViewModels(Dictionary<string, HostState> hostsState)
        {
            var allProjects = new List<Project>();
            foreach (var hostState in hostsState)
            {
                allProjects.AddRange(hostState.Value.BoincState.Projects);
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

        private async Task UpdateTasks(HostState hostState)
        {
            await hostState.BoincState.UpdateResults();            
        }

        private void UpdateTaskViewModels(HostState hostState)
        {
            foreach (Result result in hostState.BoincState.Results)
            {
                TaskViewModel taskViewModel = Tasks.FirstOrDefault(tvm => tvm.ComputerId == hostState.Id && tvm.Workunit == result.WorkunitName);

                if (taskViewModel == null)
                {
                    taskViewModel = new TaskViewModel(hostState.Id, hostState.HostName);
                    taskViewModel.Update(result, hostState.BoincState);
                    Tasks.Add(taskViewModel);
                }
                else
                {
                    taskViewModel.Update(result, hostState.BoincState);
                }
            }
        }

        private void RemoveOutdatedTaskViewModels(Dictionary<string, HostState> hostsState)
        {
            var allTasks = new List<Result>();
            foreach (var hostState in hostsState)
            {
                allTasks.AddRange(hostState.Value.BoincState.Results);
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

        private async Task UpdateTransfers(HostState hostState)
        {
            await hostState.BoincState.UpdateFileTransfers();
        }

        private void UpdateTransferViewModels(HostState hostState)
        {
            foreach (FileTransfer fileTransfer in hostState.BoincState.FileTransfers)
            {
                TransferViewModel transferVM = Transfers.FirstOrDefault(tvm => tvm.ComputerId == hostState.Id && tvm.FileName == fileTransfer.Name);

                if (transferVM == null)
                {
                    transferVM = new TransferViewModel(hostState.Id, hostState.HostName);
                    transferVM.Update(fileTransfer);
                    Transfers.Add(transferVM);
                }
                else
                {
                    transferVM.Update(fileTransfer);
                }
            }
        }

        private void RemoveOutdatedTransferViewModels(Dictionary<string, HostState> hostsState)
        {
            var allFileTransfers = new List<FileTransfer>();
            foreach (var hostState in hostsState)
            {
                allFileTransfers.AddRange(hostState.Value.BoincState.FileTransfers);
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

        private async Task UpdateMessages(HostState hostState)
        {
            var newMessages = await hostState.BoincState.GetNewMessages();
            foreach (Message newMessage in newMessages)
            {
                MessageViewModel message = new MessageViewModel(hostState.Id);
                message.Update(newMessage, hostState.HostName);
                Messages.Add(message);
            }            
        }
    }
}
