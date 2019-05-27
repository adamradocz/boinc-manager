using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace BoincManagerWeb.Pages.Projects
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public List<ProjectViewModel> Projects { get; set; }

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public void OnGet()
        {
            Projects = GetProjects(_manager.GetAllHostStates());
        }

        private List<ProjectViewModel> GetProjects(IEnumerable<HostState> hostStates)
        {
            var projects = new List<ProjectViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var project in hostState.BoincState.Projects)
                    {
                        projects.Add(new ProjectViewModel(hostState, project));
                    }
                }
            }

            return projects;
        }

    }
}
