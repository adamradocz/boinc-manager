using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BoincManagerWeb.Pages.Projects
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
            _manager.CurrentUpdateScope = BoincManager.Manager.UpdateScope.Projects;
        }

        public void OnGet()
        {
            
        }

    }
}
