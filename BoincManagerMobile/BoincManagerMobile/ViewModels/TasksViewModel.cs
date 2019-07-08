using System;
using System.Collections.ObjectModel;
using BoincManager.Models;
using BoincManagerMobile.Models;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        public ObservableCollection<ObservableTask> Tasks { get => App.Manager.Tasks; }
        public Command RefreshTasksCommand { get; }
        public Command SetFilterCommand { get; }

        public TasksViewModel()
        {
            Title = nameof(MenuItemType.Tasks);

            RefreshTasksCommand = new Command(async () => await ExecuteRefreshTasksCommand());
            SetFilterCommand = new Command(async param => await ExecuteSetFilterCommand((string)param));
        }

        async System.Threading.Tasks.Task ExecuteRefreshTasksCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await App.Manager.Update();
            IsBusy = false;
        }

        private async System.Threading.Tasks.Task ExecuteSetFilterCommand(string searchString)
        {
            App.Manager.SearchString = searchString;
            await App.Manager.Update();
        }

    }
}