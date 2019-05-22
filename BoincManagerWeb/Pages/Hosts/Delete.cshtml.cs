using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoincManager.Models;
using BoincManagerWeb.Models;

namespace BoincManagerWeb.Pages.Hosts
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly BoincManager.Manager _manager;

        public DeleteModel(ApplicationDbContext context, BoincManager.Manager manager)
        {
            _context = context;
            _manager = manager;
        }

        [BindProperty]
        public Host Host { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Host = await _context.Host.FirstOrDefaultAsync(m => m.Id == id);

            if (Host == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Host = await _context.Host.FindAsync(id);

            if (Host != null)
            {
                _context.Host.Remove(Host);
                await _context.SaveChangesAsync();

                _manager.RemoveHost(Host);
            }

            return RedirectToPage("./Index");
        }
    }
}
