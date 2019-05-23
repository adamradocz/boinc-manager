using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoincManager.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BoincManagerWeb.Pages.Hosts
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public List<HostState> Hosts { get; set; }

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            Hosts = _manager.HostsState.Values.ToList();
        }

        public async Task<IActionResult> OnPostConnectAsync(int id)
        {
            if (_manager.HostsState.ContainsKey(id))
            {
                await _manager.Connect(_manager.HostsState[id]);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDisconnect(int id)
        {
            if (_manager.HostsState.ContainsKey(id))
            {
                _manager.Disconnect(_manager.HostsState[id]);
            }

            return RedirectToPage();
        }
    }
}
