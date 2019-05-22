using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoincManagerWeb.Pages.Messages
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public List<MessageViewModel> Messages { get; set; }

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public async Task OnGetAsync()
        {
            Messages = await GetMessages(_manager.HostsState.Values);
        }

        private async Task<List<MessageViewModel>> GetMessages(IEnumerable<HostState> hostsState)
        {
            List<MessageViewModel> messagesVm = new List<MessageViewModel>();

            foreach (var hostState in hostsState)
            {
                if (hostState.Connected)
                {
                    var messages = await hostState.BoincState.GetNewMessages();
                    foreach (var message in messages)
                    {
                        messagesVm.Add(new MessageViewModel(hostState, message));
                    }
                }
            }

            return messagesVm;
        }
    }
}
