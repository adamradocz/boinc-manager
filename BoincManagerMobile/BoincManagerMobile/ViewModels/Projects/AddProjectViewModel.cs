using System;
using System.Diagnostics;
using BoincManager.Common.MVVM;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class AddProjectViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public RelayCommand AddProjectCommand { get; private set; }

        public AddProjectViewModel(INavigation navigation)
        {
            Title = "Add Project";

            _navigation = navigation;

            AddProjectCommand = new RelayCommand(async () => await ExecuteAddProjectCommand(), CanExecuteAddProjectCommand);
        }

        private async System.Threading.Tasks.Task ExecuteAddProjectCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            AddProjectCommand.RaiseCanExecuteChanged();

            try
            {
                //var reply = await App.Manager.GetHostState(hostId).RpcClient.ProjectAttachAsync();

                //MessagingCenter.Send(this, "AddProject", );
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                AddProjectCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteAddProjectCommand()
        {
            return !IsBusy;
        }
    }
}
