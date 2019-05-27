using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic; 

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

        public void OnGet()
        {
            Messages = GetMessages(_manager.GetAllHostStates());
        }

        private List<MessageViewModel> GetMessages(IEnumerable<HostState> hostStates)
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
