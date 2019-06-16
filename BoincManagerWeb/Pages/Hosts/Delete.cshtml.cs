using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoincManager.Models;

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
        public HostConnection HostConnection { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HostConnection = await _context.Host.FirstOrDefaultAsync(m => m.Id == id);

            if (HostConnection == null)
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

            HostConnection = await _context.Host.FindAsync(id);

            if (HostConnection != null)
            {
                _context.Host.Remove(HostConnection);
                await _context.SaveChangesAsync();

                _manager.RemoveHost(HostConnection.Id);
            }

            return RedirectToPage("./Index");
        }
    }
}
