using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoincManager.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
            Hosts = _manager.GetAllHostState().ToList();
        }

        public async Task<IActionResult> OnPostConnectAsync(int id)
        {
            await _manager.Connect(id);

            return RedirectToPage();
        }

        public IActionResult OnPostDisconnect(int id)
        {
            _manager.Disconnect(id);

            return RedirectToPage();
        }
    }
}
