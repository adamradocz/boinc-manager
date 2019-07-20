using BoincManagerWeb.Helpers;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace BoincManagerWeb.Hubs
{
    public class BoincInfoHub : Hub
    {
        // ConnectionId is the Key.
        private readonly static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        private readonly BoincManager.Manager _manager;
        private readonly ViewDataHelper _viewDataHelper;

        public BoincInfoHub(BoincManager.Manager manager, ViewDataHelper viewDataHelper)
        {
            _manager = manager;
            _viewDataHelper = viewDataHelper;
        }

        public override Task OnConnectedAsync()
        {
            _connections.TryAdd(Context.ConnectionId, Context.User.Identity.Name);

            // Start the Boinc background update loop if it was stopped
            if (!_manager.IsRunning)
            {
                //_manager.Start();
                Task.Run(() => _manager.Start());
            }
            else if (_manager.DelayedStopStarted)
            {
                _manager.TerminateDelayedStop();
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _connections.TryRemove(Context.ConnectionId, out _);

            // Stop the Boinc background update loop if nobody is on the site.
            //
            // 'Count' operation in the ConcurrentDictionary cause it to acquire all the locks at once.
            // Alternative method, is the lock-free enumerator: concurrentDictionary.Skip(0).Count(), because it uses LINQ
            if (_connections.Skip(0).Count() == 0)
            {
                // Manager stops after 30 seconds
                _manager.Stop(30000).ContinueWith(task => Console.WriteLine("Error: BoincInfoHub.OnDisconnectedAsync: " + task.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task GetVisitors()
        {
            await Clients.All.SendAsync("ReceiveVisitors", _connections.Skip(0).Count());
        }

        public async Task GetProjects(string searchString)
        {
            await Clients.Caller.SendAsync("ReceiveProjects", _viewDataHelper.GetProjects(_manager.GetAllHostStates(), searchString));
        }

        public async Task GetTasks(string searchString)
        {
            await Clients.Caller.SendAsync("ReceiveTasks", _viewDataHelper.GetTasks(_manager.GetAllHostStates(), searchString));
        }

        public async Task GetTransfers(string searchString)
        {
            await Clients.Caller.SendAsync("ReceiveTransfers", _viewDataHelper.GetTransfers(_manager.GetAllHostStates(), searchString));
        }

        public async Task GetMessages(string searchString)
        {
            await Clients.Caller.SendAsync("ReceiveMessages", _viewDataHelper.GetMessages(_manager.GetAllHostStates(), searchString));
        }

    }
}