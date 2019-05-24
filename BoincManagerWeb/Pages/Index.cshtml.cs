using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoincManagerWeb.Pages
{
    public class IndexModel : PageModel
    {
        public IndexViewModel Model { get; set; }

        public void OnGet()
        {
            Model = new IndexViewModel();
        }
    }
}
