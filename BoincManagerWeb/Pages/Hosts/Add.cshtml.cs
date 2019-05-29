using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BoincManager.Models;

namespace BoincManagerWeb.Pages.Hosts
{
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly BoincManager.Manager _manager;

        [BindProperty]
        public Host Host { get; set; }

        public AddModel(ApplicationDbContext context, BoincManager.Manager manager)
        {
            _context = context;
            _manager = manager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Host.Add(Host);
            await _context.SaveChangesAsync();

            _manager.AddHost(Host);
            if (Host.AutoConnect)
            {
                await _manager.Connect(Host.Id);
            }

            return RedirectToPage("./Index");
        }
    }
}