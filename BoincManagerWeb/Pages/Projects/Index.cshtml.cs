using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task OnGetAsync()
        {
            var hostStates = _manager.GetAllHostState();
            Projects = await GetProjects(hostStates);
        }

        private async Task<List<ProjectViewModel>> GetProjects(IEnumerable<HostState> hostsState)
        {
            List<ProjectViewModel> projects = new List<ProjectViewModel>();

            foreach (var hostState in hostsState)
            {
                if (hostState.Connected)
                {
                    await hostState.BoincState.UpdateProjects();

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
