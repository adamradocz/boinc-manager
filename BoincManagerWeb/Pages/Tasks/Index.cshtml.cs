using BoincManagerWeb.Helpers;
using BoincManagerWeb.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace BoincManagerWeb.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly BoincManager.Manager _manager;
        private readonly ViewDataHelper _viewDataHelper;

        public string CurrentFilter { get; set; }

        public List<Task> Tasks { get; set; }

        public IndexModel(BoincManager.Manager manager, ViewDataHelper viewDataHelper)
        {
            _manager = manager;
            _viewDataHelper = viewDataHelper;
        }

        public void OnGet(string searchString)
        {
            CurrentFilter = searchString;
            Tasks = _viewDataHelper.GetTasks(_manager.GetAllHostStates(), searchString);
        }
    }
}
