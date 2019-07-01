using BoincManager.Models;
using BoincManagerWeb.Models;
using System;
using System.Collections.Generic;

namespace BoincManagerWeb.Helpers
{
    public class ViewDataHelper
    {
        public List<Host> GetHosts(IEnumerable<HostState> hostStates, string searchString)
        {
            var hosts = new List<Host>();
            foreach (var hostState in hostStates)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    hosts.Add(new Host(hostState));
                }
                else
                {
                    var host = new Host(hostState);
                    foreach (var content in host.GetContentsForFiltering())
                    {
                        if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            // The search string is found in any of the Models's property
                            hosts.Add(host);
                            break;
                        }
                    }
                }
            }

            return hosts;
        }

        public List<Project> GetProjects(IEnumerable<HostState> hostStates, string searchString)
        {
            var projects = new List<Project>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcProject in hostState.BoincState.Projects)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            projects.Add(new Project(hostState, rpcProject));
                        }
                        else
                        {
                            var project = new Project(hostState, rpcProject);
                            foreach (var content in project.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the Models's property
                                    projects.Add(project);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return projects;
        }

        public List<Task> GetTasks(IEnumerable<HostState> hostStates, string searchString)
        {
            var tasks = new List<Task>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcResult in hostState.BoincState.Results)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            tasks.Add(new Task(hostState, rpcResult));
                        }
                        else
                        {
                            var task = new Task(hostState, rpcResult);
                            foreach (var content in task.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the Models's property
                                    tasks.Add(task);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return tasks;
        }

        public List<Transfer> GetTransfers(IEnumerable<HostState> hostStates, string searchString)
        {
            var transfers = new List<Transfer>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcFileTransfer in hostState.BoincState.FileTransfers)
                    {                        
                        if (string.IsNullOrEmpty(searchString))
                        {
                            transfers.Add(new Transfer(hostState, rpcFileTransfer));
                        }
                        else
                        {
                            var transfer = new Transfer(hostState, rpcFileTransfer);
                            foreach (var content in transfer.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the Models's property
                                    transfers.Add(transfer);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return transfers;
        }

        public List<Message> GetMessages(IEnumerable<HostState> hostStates, string searchString)
        {
            var messages = new List<Message>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcMessage in hostState.BoincState.Messages)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            messages.Add(new Message(hostState, rpcMessage));
                        }
                        else
                        {
                            var message = new Message(hostState, rpcMessage);
                            foreach (var content in message.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the Models's property
                                    messages.Add(message);
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
