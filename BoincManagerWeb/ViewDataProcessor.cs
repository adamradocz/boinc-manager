using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using System.Collections.Generic;

namespace BoincManagerWeb
{
    public class ViewDataProcessor
    {
        public List<ProjectViewModel> GetProjects(IEnumerable<HostState> hostStates)
        {
            var projects = new List<ProjectViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var project in hostState.BoincState.Projects)
                    {
                        projects.Add(new ProjectViewModel(hostState, project));
                    }
                }
            }

            return projects;
        }

        public List<TaskViewModel> GetTasks(IEnumerable<HostState> hostStates)
        {
            var tasks = new List<TaskViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var result in hostState.BoincState.Results)
                    {
                        tasks.Add(new TaskViewModel(hostState, result));
                    }
                }
            }

            return tasks;
        }

        public List<TransferViewModel> GetTransfers(IEnumerable<HostState> hostStates)
        {
            var transfers = new List<TransferViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var fileTransfer in hostState.BoincState.FileTransfers)
                    {
                        transfers.Add(new TransferViewModel(hostState, fileTransfer));
                    }
                }
            }

            return transfers;
        }

        public List<MessageViewModel> GetMessages(IEnumerable<HostState> hostStates)
        {
            var messagesVm = new List<MessageViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var message in hostState.BoincState.Messages)
                    {
                        messagesVm.Add(new MessageViewModel(hostState, message));
                    }
                }
            }

            return messagesVm;
        }
    }
}
