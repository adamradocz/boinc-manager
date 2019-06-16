using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BoincManager.Models;

namespace BoincManagerWeb.Pages.Hosts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly BoincManager.Manager _manager;

        public EditModel(ApplicationDbContext context, BoincManager.Manager manager)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(HostConnection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _manager.UpdateHost(HostConnection);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HostExists(HostConnection.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool HostExists(int id)
        {
            return _context.Host.Any(e => e.Id == id);
        }
    }
}
