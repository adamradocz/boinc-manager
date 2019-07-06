using BoincManager.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TaskDetailViewModel : BaseViewModel
    {
        public ObservableTask Task { get; set; }
        public TaskDetailViewModel(ObservableTask task)
        {
            Title = task.Application;
            Task = task;
        }
    }
}
