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
        private readonly BoincManagerWeb.Models.ApplicationDbContext _context;

        public IndexModel(BoincManagerWeb.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Host> Host { get;set; }

        public async Task OnGetAsync()
        {
            Host = await _context.Host.ToListAsync();
        }
    }
}
