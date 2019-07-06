using System.Collections.ObjectModel;

using Xamarin.Forms;
using BoincManager.Models;
using BoincManagerMobile.Models;
using BoincManagerMobile.Views;

namespace BoincManagerMobile.ViewModels
{
    public class ProjectsViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public ObservableCollection<ObservableProject> Projects { get => App.Manager.Projects; }
        public Command RefreshProjectsCommand { get; }
        public Command AddProjectsCommand { get; }

        public ProjectsViewModel(INavigation navigation)
        {
            Title = nameof(MenuItemType.Projects);

            _navigation = navigation;

            RefreshProjectsCommand = new Command(async () => await ExecuteRefreshProjectsCommandAsync());
            AddProjectsCommand = new Command(async () => await ExecuteAddProjectsCommand());
        }

        async System.Threading.Tasks.Task ExecuteRefreshProjectsCommandAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await App.Manager.Update();            
            IsBusy = false;
        }

        private async System.Threading.Tasks.Task ExecuteAddProjectsCommand()
        {
            await _navigation.PushAsync(new AddProjectPage());
        }
    }
}