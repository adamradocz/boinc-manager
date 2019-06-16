using BoincManagerWeb.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace BoincManagerWeb.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly BoincManager.Manager _manager;
        private readonly ViewDataProcessor _viewDataProcessor;

        public string CurrentFilter { get; set; }

        public List<Project> Projects { get; set; }

        public IndexModel(BoincManager.Manager manager, ViewDataProcessor viewDataProcessor)
        {
            _manager = manager;
            _viewDataProcessor = viewDataProcessor;
        }

        public void OnGet(string searchString)
        {
            CurrentFilter = searchString;
            Projects = _viewDataProcessor.GetProjects(_manager.GetAllHostStates(), searchString);
        }
    }
}
