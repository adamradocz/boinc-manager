using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoincManager.Models;
using BoincManager.ViewModels;
using BoincRpc;

namespace BoincManager
{
    public class Manager
    {
        public Dictionary<int, HostState> HostsState { get; } // Key is the host's Id
        public List<HostViewModel> Hosts { get; }
        public List<ProjectViewModel> Projects { get; }
        public List<TaskViewModel> Tasks { get; }
        public List<TransferViewModel> Transfers { get; }
        public List<MessageViewModel> Messages { get; }

        public enum UpdateScope
        {
            None,
            All,
            Hosts,
            Projects,
            Tasks,
            Transfers,
            Messages,
        }
        public UpdateScope CurrentUpdateScope { get; set; }

        public Manager()
        {
            HostsState = new Dictionary<int, HostState>();
            Hosts = new List<HostViewModel>();            
            Projects = new List<ProjectViewModel>();
            Tasks = new List<TaskViewModel>();
            Transfers = new List<TransferViewModel>();
            Messages = new List<MessageViewModel>();
        }

        public async Task Start(IList<Host> hostsModel)
        {
            // Connect to all the hosts stored in the database.
            foreach (var host in hostsModel)
            {
                await AddHost(host);
            }
            // await Utils.ParallelForEachAsync(hosts, AddHost); // Connect to all the hosts in parallel, instead of sequential order

#pragma warning disable CS4014
            StartBoincUpdateLoop(CancellationToken.None);
#pragma warning restore CS4014
        }

        public async Task AddHost(Host hostModel)
        {
            var hostVm = new HostViewModel(hostModel);
            await Connect(hostVm);
            Hosts.Add(hostVm);
        }

        public void RemoveHost(Host hostModel)
        {
            // Remove the host from Models
            if (HostsState.ContainsKey(hostModel.Id))
            {
                HostsState[hostModel.Id].Close();
                HostsState[hostModel.Id].Dispose();
                HostsState.Remove(hostModel.Id);
            }            

            // Remove the host from ViewModels
            for (int i = 0; i < Hosts.Count; i++)
            {
                if (Hosts[i].Id == hostModel.Id)
                {
                    Hosts.RemoveAt(i);
                    break;
                }
            }            
        }

        public void UpdateHost(Host hostModel)
        {
            for (int i = 0; i < Hosts.Count; i++)
            {
                if (Hosts[i].Id == hostModel.Id)
                {
                    Hosts[i].Update(hostModel);
                    break;
                }
            }
        }

        private async Task Connect(HostViewModel hostVm)
        {
            if (string.IsNullOrWhiteSpace(hostVm.IpAddress) || string.IsNullOrEmpty(hostVm.Password) || hostVm.Connected)
            {
                return;
            }

            hostVm.Status = $"Connecting...";

            try
            {
                var hostState = new HostState(hostVm.Id, hostVm.Name);

                // Connecting to host
                await hostState.RpcClient.ConnectAsync(hostVm.IpAddress, hostVm.Port);
                hostState.Authorized = await hostState.RpcClient.AuthorizeAsync(hostVm.Password);

                if (hostState.Authorized)
                {
                    hostVm.Status = "Connected. Updating...";

                    HostsState.Add(hostVm.Id, hostState);

                    // Fetch the data and update the components
                    await hostState.BoincState.UpdateAll();
                    await hostVm.FirstUpdateOnConnect(hostState);
                    UpdateProjectViewModels(hostState);
                    UpdateTaskViewModels(hostState);
                    UpdateTransferViewModels(hostState);
                    await UpdateMessages(hostState);
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
        
        private async Task StartBoincUpdateLoop(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(2000, cancellationToken);

                if (CurrentUpdateScope == UpdateScope.None)
                {
                    return;
                }

                // TODO
                // Update in parallel (It seems something is not thread-safe, so until it uses a normal loop.)
                //await Utils.ParallelForEachAsync(filteredHosts.Values, Update);
                foreach (var hostState in HostsState)
                {
                    await Update(hostState.Value);
                }

                switch (CurrentUpdateScope)
                {
                    case UpdateScope.Projects:
                        RemoveOutdatedProjectViewModels(HostsState);
                        break;
                    case UpdateScope.Tasks:
                        RemoveOutdatedTaskViewModels(HostsState);
                        break;
                    case UpdateScope.Transfers:
                        RemoveOutdatedTransferViewModels(HostsState);
                        break;
                    case UpdateScope.Messages:
                        break;
                }
            }
        }

        /// <summary>
        /// Update the Models and the ViewModels, but only on the visible tabs.
        /// </summary>
        /// <returns></returns>
        private async Task Update(HostState hostState)
        {
            try
            {
                switch (CurrentUpdateScope)
                {
                    case UpdateScope.Projects:
                        await UpdateProjects(hostState);
                        UpdateProjectViewModels(hostState);
                        break;
                    case UpdateScope.Tasks:
                        await UpdateTasks(hostState);
                        UpdateTaskViewModels(hostState);
                        break;
                    case UpdateScope.Transfers:
                        await UpdateTransfers(hostState);
                        UpdateTransferViewModels(hostState);
                        break;
                    case UpdateScope.Messages:
                        await UpdateMessages(hostState);
                        break;
                }
            }
            catch (Exception e)
            {
                var hostVm = Hosts.FirstOrDefault(c => c.Id == hostState.Id);
                if (hostVm != null)
                    hostVm.Status = $"Error: {e.Message}";
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

        private void RemoveOutdatedProjectViewModels(Dictionary<int, HostState> hostsState)
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
                TaskViewModel taskViewModel = Tasks.FirstOrDefault(tvm => tvm.HostId == hostState.Id && tvm.Workunit == result.WorkunitName);

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

        private void RemoveOutdatedTaskViewModels(Dictionary<int, HostState> hostsState)
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
                TransferViewModel transferVM = Transfers.FirstOrDefault(tvm => tvm.HostId == hostState.Id && tvm.FileName == fileTransfer.Name);

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

        private void RemoveOutdatedTransferViewModels(Dictionary<int, HostState> hostsState)
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
