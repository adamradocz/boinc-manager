using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoincManagerWeb.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        public readonly BoincManager.Manager _manager;

        public List<TaskViewModel> Tasks { get; set; }

        public IndexModel(BoincManager.Manager manager)
        {
            _manager = manager;
        }

        public async Task OnGetAsync()
        {
            var hostStates = _manager.GetAllHostState();
            Tasks = await GetTasks(hostStates);
        }

        private async Task<List<TaskViewModel>> GetTasks(IEnumerable<HostState> hostsState)
        {
            List<TaskViewModel> tasks = new List<TaskViewModel>();

            foreach (var hostState in hostsState)
            {
                if (hostState.Connected)
                {
                    await hostState.BoincState.UpdateResults();

                    foreach (var result in hostState.BoincState.Results)
                    {
                        tasks.Add(new TaskViewModel(hostState, result));
                    }
                }
            }

            return tasks;
        }
    }
}
