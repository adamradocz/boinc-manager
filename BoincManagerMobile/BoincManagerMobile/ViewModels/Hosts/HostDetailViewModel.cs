using BoincManager.Common.MVVM;
using BoincManager.Models;
using BoincManagerMobile.Models;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace BoincManagerMobile.ViewModels
{
    public class HostDetailViewModel : BaseViewModel
    {
        private INavigation _navigation;

        public Host Host { get; set; }
        public RelayCommand RemoveHostCommand { get; private set; }

        public HostDetailViewModel(Host host, INavigation navigation)
        {
            Title = host.Name;
            Host = host;

            _navigation = navigation;

            RemoveHostCommand = new RelayCommand(async () => await ExecuteRemoveHostCommand(), CanExecuteRemoveHostCommand);
        }

        private async System.Threading.Tasks.Task ExecuteRemoveHostCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            RemoveHostCommand.RaiseCanExecuteChanged();

            try
            {
                using (var context = new ApplicationDbContext(Utils.Storage.GetDbContextOptions()))
                {
                    var hostConnection = await context.Host.FindAsync(Host.Id);
                    if (hostConnection != null)
                    {
                        context.Host.Remove(hostConnection);
                        await context.SaveChangesAsync();

                        App.Manager.RemoveHost(Host.Id);

                        MessagingCenter.Send(this, "RemoveHost", Host);
                    }
                }

                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                RemoveHostCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteRemoveHostCommand()
        {
            return !IsBusy;
        }
    }
}
