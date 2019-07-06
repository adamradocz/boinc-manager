using BoincManager.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        public BoincTask Task { get; set; }
        public TaskDetailViewModel(BoincTask task)
        {
            Title = task.Application;
            Task = task;
        }
    }
}
