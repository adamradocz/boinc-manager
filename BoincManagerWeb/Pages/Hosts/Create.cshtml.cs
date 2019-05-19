using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BoincManager.Models;
using BoincManagerWeb.Models;

namespace BoincManagerWeb.Pages.Hosts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly BoincManager.Manager _manager;

        public CreateModel(ApplicationDbContext context, BoincManager.Manager manager)
        {
            _context = context;
            _manager = manager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Host Host { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Host.Add(Host);
            await _context.SaveChangesAsync();

            await _manager.AddHost(Host);

            return RedirectToPage("./Index");
        }
    }
}