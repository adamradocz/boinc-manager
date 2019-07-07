using BoincManager.Common.MVVM;
using BoincManager.Models;
using BoincManagerMobile.Models;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class AddHostViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public HostConnection HostConnection { get; set; }
        public RelayCommand AddHostCommand { get; private set; }

        public AddHostViewModel(INavigation navigation)
        {
            Title = "Add Host";

            HostConnection = new HostConnection();

            _navigation = navigation;

            AddHostCommand = new RelayCommand(async () => await ExecuteAddHostCommand(), CanExecuteAddHostCommand);
        }

        private async System.Threading.Tasks.Task ExecuteAddHostCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            AddHostCommand.RaiseCanExecuteChanged();

            try
            {
                using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
                {
                    await context.AddAsync(HostConnection);
                    await context.SaveChangesAsync();
                }

                App.Manager.AddHost(HostConnection);
                if (HostConnection.AutoConnect)
                {
                    await App.Manager.Connect(HostConnection.Id);
                }

                MessagingCenter.Send(this, "AddHost", new ObservableHost(App.Manager.GetHostState(HostConnection.Id)));
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                AddHostCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteAddHostCommand()
        {
            return !IsBusy
                && !string.IsNullOrEmpty(HostConnection.Name)
                && !string.IsNullOrEmpty(HostConnection.IpAddress)
                && !string.IsNullOrEmpty(HostConnection.Password)
                && HostConnection.Port > 0
                && HostConnection.Port <= 65535;
        }
    }
}
