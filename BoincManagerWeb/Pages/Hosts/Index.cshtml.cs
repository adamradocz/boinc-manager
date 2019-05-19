using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoincManager.Models;
using BoincManagerWeb.Models;
using BoincManagerWeb.ViewModels;

namespace BoincManagerWeb.Pages.Hosts
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Host> Hosts { get;set; }
        public IList<HostViewModel> HostVms { get; set; }

        public async Task OnGetAsync()
        {
            Hosts = await _context.Host.ToListAsync();           
        }
    }
}
