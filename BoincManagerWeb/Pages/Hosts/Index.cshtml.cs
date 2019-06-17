using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BoincManagerWeb.Models;
using BoincManagerWeb.Helpers;

namespace BoincManagerWeb.Pages.Hosts
{
    public class IndexModel : PageModel
    {
        private readonly BoincManager.Manager _manager;
        private readonly ViewDataHelper _viewDataHelper;

        public List<Host> Hosts { get; set; }

        public string CurrentFilter { get; set; }
        public string NameSort { get; set; }
        public string IpAddressSort { get; set; }
        public string AutoConnectSort { get; set; }
        public string StatusSort { get; set; }        

        public IndexModel(BoincManager.Manager manager, ViewDataHelper viewDataProcessor)
        {
            _manager = manager;
            _viewDataHelper = viewDataProcessor;
        }

        public void OnGet(string searchString, string sortOrder)
        {
            CurrentFilter = searchString;

            if (string.IsNullOrEmpty(sortOrder))
                NameSort = "name_asc";

            NameSort = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            IpAddressSort = sortOrder == "ipaddress_asc" ? "ipaddress_desc" : "ipaddress_asc";
            AutoConnectSort = sortOrder == "autoconnect_asc" ? "autoconnect_desc" : "autoconnect_asc";
            StatusSort = sortOrder == "status_asc" ? "status_desc" : "status_asc";

            IQueryable<Host> hostStateIQ = _viewDataHelper.GetHosts(_manager.GetAllHostStates(), searchString).AsQueryable();

            switch (sortOrder)
            {
                case "name_asc":
                    hostStateIQ = hostStateIQ.OrderBy(h => h.Name);
                    break;
                case "name_desc":
                    hostStateIQ = hostStateIQ.OrderByDescending(h => h.Name);
                    break;
                case "ipaddress_asc":
                    hostStateIQ = hostStateIQ.OrderBy(h => h.IpAddress);
                    break;
                case "ipaddress_desc":
                    hostStateIQ = hostStateIQ.OrderByDescending(h => h.IpAddress);
                    break;
                case "autoconnect_asc":
                    hostStateIQ = hostStateIQ.OrderBy(h => h.AutoConnect);
                    break;
                case "autoconnect_desc":
                    hostStateIQ = hostStateIQ.OrderByDescending(h => h.AutoConnect);
                    break;
                case "status_asc":
                    hostStateIQ = hostStateIQ.OrderBy(h => h.Status);
                    break;
                case "status_desc":
                    hostStateIQ = hostStateIQ.OrderByDescending(h => h.Status);
                    break;
                default:
                    hostStateIQ = hostStateIQ.OrderBy(h => h.Name);
                    break;
            }

            Hosts = hostStateIQ.ToList();
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
