using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoincManager.Models;
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
            Hosts = _manager.HostsState.Values.ToList();
        }

    }
}
