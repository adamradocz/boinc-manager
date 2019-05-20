using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoincManager.Models;
using BoincManagerWeb.Models;

namespace BoincManagerWeb.Pages.Hosts
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
            _manager.CurrentUpdateScope = BoincManager.Manager.UpdateScope.Hosts;
        }

        public void OnGet()
        {
        }

    }
}
