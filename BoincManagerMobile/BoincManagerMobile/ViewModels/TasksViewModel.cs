using System.Collections.ObjectModel;
using BoincManager.Models;
using BoincManagerMobile.Models;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        public ObservableCollection<BoincTask> Tasks { get => App.Manager.Tasks; }
        public Command RefreshTasksCommand { get; }

        public TasksViewModel()
        {
            Title = nameof(MenuItemType.Tasks);

            RefreshTasksCommand = new Command(async () => await ExecuteRefreshTasksCommandAsync());
        }

        async System.Threading.Tasks.Task ExecuteRefreshTasksCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await App.Manager.Update();
            IsBusy = false;
        }

    }
}