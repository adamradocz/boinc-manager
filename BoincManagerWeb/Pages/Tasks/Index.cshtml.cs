using BoincManager.Models;
using BoincManagerWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

        public void OnGet()
        {
            Tasks = GetTasks(_manager.GetAllHostStates());
        }

        private List<TaskViewModel> GetTasks(IEnumerable<HostState> hostStates)
        {
            var tasks = new List<TaskViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
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
