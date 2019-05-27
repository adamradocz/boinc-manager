using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace BoincManagerWeb.Hubs
{
    public class BoincInfoHub : Hub
    {
        // The Key  is the ConnectionId
        private readonly static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

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
            // 'Count' operation in the ConcurrentDictionary cause it to acquire all the locks at once. Alternative method, is the lock-free enumerator: concurrentDictionary.Skip(0).Count()
            if (_connections.Skip(0).Count() == 0)
            {
                _manager.Stop();
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message, _connections.Count);
        }
    }
}