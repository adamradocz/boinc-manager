using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        public Task Task { get; set; }
        public TaskDetailViewModel(Task task)
        {
            Title = task.Application;
            Task = task;
        }
    }
}
