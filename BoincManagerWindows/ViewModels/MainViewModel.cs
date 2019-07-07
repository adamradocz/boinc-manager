using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using BoincManager.Common.MVVM;
using BoincManager.Models;
using BoincManagerWindows.Models;

namespace BoincManagerWindows.ViewModels
{
    class MainViewModel : BindableBase, IDisposable
    {
        bool disposed = false;

        private readonly BoincManager.Manager _manager;

        public List<HostGorup> ComputerGroups { get; }
        public ObservableCollection<ObservableHost> Computers { get => _manager.Hosts; }
        public ObservableCollection<ObservableProject> Projects { get => _manager.Projects; }
        public ObservableCollection<ObservableTask> Tasks { get => _manager.Tasks; }
        public ObservableCollection<ObservableTransfer> Transfers { get => _manager.Transfers; }
        public ObservableCollection<ObservableMessage> Messages { get => _manager.Messages; }

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
            StartBoincManager();

            ComputerGroups = new List<HostGorup>();

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
            ComputerGroups.Add(new HostGorup("All")
            {
                Members = Computers.ToList()
            });
        }

        private void ExecuteCloseCommand()
        {
            Dispose();
            Application.Current.Shutdown();
        }

        private async void ExecuteCloseAndStopBoincCommand()
        {
            if (MessageBox.Show($"Stop BOINC on localhost. Are you sure?", "Stop BOINC and close BOINC Manager", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            var hostStates = _manager.GetAllHostStates();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected && hostState.IsLocalhost)
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
                if (hostState.Connected && hostState.IsLocalhost)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteRunBenchmarksCommand()
        {
            foreach(ObservableHost selectedComputer in SelectedComputers)
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
            var host = new HostConnection("New host", "localhost", "123");
            using (var db = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                db.Host.Add(host);
                await db.SaveChangesAsync();
            }

            _manager.AddHost(host);
            //Computers.Add(new Host(_manager.GetHostState(host.Id)));
        }
        
        private async void ExecuteRemoveComputerCommand()
        {
            string messageBoxText = SelectedComputers.Count == 1
                ? string.Format("Removing computer {0}. Are you sure?", ((ObservableHost)SelectedComputers[0]).Name)
                : string.Format("Removing {0} computers. Are you sure?", SelectedComputers.Count);

            if (MessageBox.Show(messageBoxText, "Removing computer(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            // Remove the selected computers from Model
            List<int> removableComputerIds = new List<int>();
            using (var db = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                foreach (ObservableHost selectedHost in SelectedComputers)
                {
                    var removableHost = await db.Host.FindAsync(selectedHost.Id);
                    if (removableHost != null)
                    {
                        db.Host.Remove(removableHost);
                    }
                    
                    removableComputerIds.Add(selectedHost.Id);
                }

                await db.SaveChangesAsync();
            }

            // Remove the selected computers from ViewModel
            foreach (var removableComputerId in removableComputerIds)
            {
                _manager.RemoveHost(removableComputerId);
            }            
        }

        private async void ExecuteConnectComputerCommand()
        {
            foreach (ObservableHost computer in SelectedComputers)
            {
                if (!computer.Connected)
                {
                    await _manager.Connect(computer.Id);
                }
            }

            await _manager.Update();
        }

        private bool CanExecuteConnectComputerCommand()
        {
            if (SelectedComputers != null && SelectedComputers.Count != 0)
            {
                foreach (ObservableHost computer in SelectedComputers)
                {
                    if (!computer.Connected)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private async void ExecuteDisconnectComputerCommand()
        {
            foreach (ObservableHost computer in SelectedComputers)
            {
                if (computer.Connected)
                {
                    _manager.Disconnect(computer.Id);
                }
            }

            await _manager.Update();
        }

        private bool CanExecuteDisconnectComputerCommand()
        {
            if (SelectedComputers != null && SelectedComputers.Count != 0)
            {
                foreach (ObservableHost computer in SelectedComputers)
                {
                    if (computer.Connected)
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
            foreach (ObservableHost selectedComputer in SelectedComputers)
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
                await _manager.Update();
            }
        }

        private async void ExecuteUpdateProjectCommand()
        {
            foreach (ObservableProject selectedProject in SelectedProjects)
            {
                await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.RpcProject, BoincRpc.ProjectOperation.Update);
            }

            await _manager.Update();
        }

        private bool IsProjectSelected()
        {
            return SelectedProjects != null && SelectedProjects.Count != 0;
        }

        private async void ExecuteSuspendProjectCommand()
        {
            foreach (ObservableProject selectedProject in SelectedProjects)
            {
                if (!selectedProject.RpcProject.Suspended)
                {
                    await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.RpcProject, BoincRpc.ProjectOperation.Suspend);
                }
            }

            await _manager.Update();
        }

        private async void ExecuteNoNewTasksProjectCommand()
        {
            foreach (ObservableProject selectedProject in SelectedProjects)
            {
                if (!selectedProject.RpcProject.DontRequestMoreWork)
                {
                    await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.RpcProject, BoincRpc.ProjectOperation.NoMoreWork);
                }
            }

            await _manager.Update();
        }

        private async void ExecuteResetProjectCommand()
        {
            string messageBoxText = SelectedProjects.Count == 1
                ? $"Resetting project {((ObservableProject)SelectedProjects[0]).Name}. Are you sure?"
                : $"Resetting {SelectedProjects.Count} projects. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Resetting project(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (ObservableProject selectedProject in SelectedProjects)
            {
                await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.RpcProject, BoincRpc.ProjectOperation.Reset);
            }

            await _manager.Update();
        }

        private async void ExecuteDetachProjectCommand()
        {
            string messageBoxText = SelectedProjects.Count == 1
                ? $"Detaching project {((ObservableProject)SelectedProjects[0]).Name}. Are you sure?"
                : $"Detaching {SelectedProjects.Count} projects. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Detaching project(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (ObservableProject selectedProject in SelectedProjects)
            {
                await _manager.GetHostState(selectedProject.HostId).RpcClient.PerformProjectOperationAsync(selectedProject.RpcProject, BoincRpc.ProjectOperation.Detach);
            }

            await _manager.Update();
        }        

        private void ExecuteShowGraphicsCommand()
        {
            foreach (ObservableTask selectedTask in SelectedTasks)
            {
                if (_manager.GetHostState(selectedTask.HostId).IsLocalhost && selectedTask.RpcResult.GraphicsAvailable)
                {
                    GraphicsAppLauncher.StartGraphicsAppOrBringToTop(selectedTask.RpcResult.GraphicsExecPath, selectedTask.RpcResult.SlotPath);
                }
            }
        }

        private bool CanExecuteShowGraphicsCommand()
        {
            if (SelectedTasks == null || SelectedTasks.Count == 0)
                return false;

            foreach (ObservableTask selectedTask in SelectedTasks)
            {
                if (_manager.GetHostState(selectedTask.HostId).IsLocalhost && selectedTask.RpcResult.GraphicsAvailable)
                {
                    return true;
                }
            }

            return false;
        }

        private async void ExecuteSuspendTaskCommand()
        {
            foreach (ObservableTask selectedTask in SelectedTasks)
            {
                if (selectedTask.RpcResult.Suspendable && !selectedTask.RpcResult.Suspended)
                {
                    await _manager.GetHostState(selectedTask.HostId).RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, BoincRpc.ResultOperation.Suspend);
                }
                else if (selectedTask.RpcResult.Suspended)
                {
                    await _manager.GetHostState(selectedTask.HostId).RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, BoincRpc.ResultOperation.Resume);
                }
            }

            await _manager.Update();
        }

        private bool CanExecuteSuspendTaskCommand()
        {
            if (SelectedTasks == null || SelectedTasks.Count == 0)
                return false;

            foreach (ObservableTask selectedTask in SelectedTasks)
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
                ? $"Aborting task {((ObservableTask)SelectedTasks[0]).Workunit}. Are you sure?"
                : $"Aborting {SelectedTasks.Count} tasks. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Aborting task(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;
            
            foreach (ObservableTask selectedTask in SelectedTasks)
            {
                if (selectedTask.RpcResult.Abortable)
                {
                    await _manager.GetHostState(selectedTask.HostId).RpcClient.PerformResultOperationAsync(selectedTask.RpcResult, BoincRpc.ResultOperation.Abort);
                }
            }

            await _manager.Update();
        }

        private bool CanExecuteAbortTaskCommand()
        {
            if (SelectedTasks == null || SelectedTasks.Count == 0)
                return false;

            foreach (ObservableTask selectedTask in SelectedTasks)
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
            foreach (ObservableTransfer selectedTransfer in SelectedTransfers)
            {
                await _manager.GetHostState(selectedTransfer.HostId).RpcClient.PerformFileTransferOperationAsync(selectedTransfer.FileTransfer, BoincRpc.FileTransferOperation.Retry);
            }

            await _manager.Update();
        }

        private bool CanExecuteRetryTransferCommand()
        {
            return SelectedTransfers != null && SelectedTransfers.Count != 0;
        }

        private async void ExecuteAbortTransferCommand()
        {
            string messageBoxText = SelectedTransfers.Count == 1
                ? $"Aborting transfer {((ObservableTransfer)SelectedTransfers[0]).FileName}. Are you sure?"
                : $"Aborting {SelectedTransfers.Count} transfer. Are you sure?";
            if (MessageBox.Show(messageBoxText, "Aborting transfer(s)", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            foreach (ObservableTransfer selectedTransfer in SelectedTransfers)
            {
                await _manager.GetHostState(selectedTransfer.HostId).RpcClient.PerformFileTransferOperationAsync(selectedTransfer.FileTransfer, BoincRpc.FileTransferOperation.Abort);
            }

            await _manager.Update();
        }

        private bool CanExecuteAbortTransferCommand()
        {
            return SelectedTransfers != null && SelectedTransfers.Count != 0;
        }

        private void ExecuteCopyMessagesCommand()
        {
            var text = new StringBuilder();
            foreach (ObservableMessage selectedMessage in SelectedMessages)
            {
                text.AppendLine(selectedMessage.MessageBody);
            }

            Clipboard.SetText(text.ToString());
        }

        private bool CanExecuteCopyMessagesCommand()
        {
            return SelectedMessages != null && SelectedMessages.Count != 0;
        }

        public void StartBoincManager()
        {
            status = "Loading database...";
            // Initialize the application
            using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
            {
                BoincManager.Utils.InitializeApplication(context, _manager, true);
            }

            // Start the Boinc Manager
            status = "Connecting...";
            _manager.Start();
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
                _manager.Dispose();
            }

            disposed = true;
        }

    }
}
