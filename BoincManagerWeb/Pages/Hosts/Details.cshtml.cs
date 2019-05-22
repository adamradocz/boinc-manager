using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoincManager.Models;

namespace BoincManagerWeb.Pages.Hosts
{
    public class DetailsModel : PageModel
    {
        private readonly BoincManager.Manager _manager;

        public DetailsModel(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public HostState Host { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Host = _manager.HostsState[id.GetValueOrDefault()];

            if (Host == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
