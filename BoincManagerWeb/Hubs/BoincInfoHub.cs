using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoincManagerWeb.Hubs
{
    public class BoincInfoHub : Hub
    {
        // ConnectionId is the Key.
        private readonly static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        private static List<TaskViewModel> _tasks = new List<TaskViewModel>();

        private readonly BoincManager.Manager _manager;

        public BoincInfoHub(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public override Task OnConnectedAsync()
        {
            _connections.TryAdd(Context.ConnectionId, Context.User.Identity.Name);

            // Start the Boinc background update loop if it was stopped
            if (!_manager.IsRunning)
            {
                _manager.Start();
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryRemove(Context.ConnectionId, out _);

            // Stop the Boinc background update loop if nobody is on the site.
            // 'Count' operation in the ConcurrentDictionary cause it to acquire all the locks at once.
            // Alternative method, is the lock-free enumerator: concurrentDictionary.Skip(0).Count(), because it uses LINQ
            if (_connections.Count() == 0)
            {
                // TODO - It can't start again, if it's stopped.
                //_manager.Stop();
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, _connections.Count);
        }

        public async Task GetTasks()
        {
            _tasks = GetTasksVm(_manager.GetAllHostStates());
            await Clients.Caller.SendAsync("ReceiveTasks", _tasks, _manager.updateNumber, _manager.IsRunning);
        }
        
        public List<TaskViewModel> GetTasksVm(IEnumerable<HostState> hostStates)
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
    }
}