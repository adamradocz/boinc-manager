using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using System;
using System.Collections.Generic;

namespace BoincManagerWeb
{
    public class ViewDataProcessor
    {
        public List<HostViewModel> GetHosts(IEnumerable<HostState> hostStates, string searchString)
        {
            var hosts = new List<HostViewModel>();
            foreach (var hostState in hostStates)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    hosts.Add(new HostViewModel(hostState));
                }
                else
                {
                    var hostVM = new HostViewModel(hostState);
                    foreach (var content in hostVM.GetContentsForFiltering())
                    {
                        if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            // The search string is found in any of the VM's property
                            hosts.Add(hostVM);
                            break;
                        }
                    }
                }
            }

            return hosts;
        }

        public List<ProjectViewModel> GetProjects(IEnumerable<HostState> hostStates, string searchString)
        {
            var projects = new List<ProjectViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var project in hostState.BoincState.Projects)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            projects.Add(new ProjectViewModel(hostState, project));
                        }
                        else
                        {
                            var projectVM = new ProjectViewModel(hostState, project);
                            foreach (var content in projectVM.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    projects.Add(projectVM);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return projects;
        }

        public List<TaskViewModel> GetTasks(IEnumerable<HostState> hostStates, string searchString)
        {
            var tasks = new List<TaskViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var result in hostState.BoincState.Results)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            tasks.Add(new TaskViewModel(hostState, result));
                        }
                        else
                        {
                            var taskVM = new TaskViewModel(hostState, result);
                            foreach (var content in taskVM.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    tasks.Add(taskVM);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return tasks;
        }

        public List<TransferViewModel> GetTransfers(IEnumerable<HostState> hostStates, string searchString)
        {
            var transfers = new List<TransferViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var fileTransfer in hostState.BoincState.FileTransfers)
                    {                        
                        if (string.IsNullOrEmpty(searchString))
                        {
                            transfers.Add(new TransferViewModel(hostState, fileTransfer));
                        }
                        else
                        {
                            var transferVM = new TransferViewModel(hostState, fileTransfer);
                            foreach (var content in transferVM.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    transfers.Add(transferVM);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return transfers;
        }

        public List<MessageViewModel> GetMessages(IEnumerable<HostState> hostStates, string searchString)
        {
            var messages = new List<MessageViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var message in hostState.BoincState.Messages)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            messages.Add(new MessageViewModel(hostState, message));
                        }
                        else
                        {
                            var messageVM = new MessageViewModel(hostState, message);
                            foreach (var content in messageVM.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    messages.Add(messageVM);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return messages;
        }
    }
}
