using System;
using System.Diagnostics;
using BoincManager.Common.MVVM;
using BoincManager.Models;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class ProjectDetailViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public ObservableProject Project { get; set; }

        public RelayCommand RemoveProjectCommand { get; private set; }

        public ProjectDetailViewModel(ObservableProject project, INavigation navigation)
        {
            Title = project.Name;
            Project = project;

            _navigation = navigation;

            RemoveProjectCommand = new RelayCommand(async () => await ExecuteRemoveProjectCommand(), CanExecuteRemoveProjectCommand);
        }

        private async System.Threading.Tasks.Task ExecuteRemoveProjectCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            RemoveProjectCommand.RaiseCanExecuteChanged();

            try
            {
                await App.Manager.GetHostState(Project.HostId).RpcClient.PerformProjectOperationAsync(Project.RpcProject, BoincRpc.ProjectOperation.Detach);
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                RemoveProjectCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteRemoveProjectCommand()
        {
            return !IsBusy;
        }
    }
}
