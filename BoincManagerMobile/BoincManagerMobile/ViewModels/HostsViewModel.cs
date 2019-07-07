using System.Collections.ObjectModel;

using Xamarin.Forms;
using BoincManager.Models;
using BoincManagerMobile.Models;
using BoincManagerMobile.Views;

namespace BoincManagerMobile.ViewModels
{
    public class HostsViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        public ObservableCollection<ObservableHost> Hosts { get => App.Manager.Hosts; }
        public Command RefreshHostsCommand { get; }
        public Command AddHostsCommand { get; }

        public HostsViewModel(INavigation navigation)
        {
            Title = nameof(MenuItemType.Hosts);

            _navigation = navigation;

            RefreshHostsCommand = new Command(async () => await ExecuteRefreshHostsCommand());
            AddHostsCommand = new Command(async () => await ExecuteAddHostsCommandAsync());

            MessagingCenter.Subscribe<AddHostPage, ObservableHost>(this, "AddHost", (obj, host) =>
            {
                //_hosts.Add(host);
            });

            MessagingCenter.Subscribe<HostDetailPage, ObservableHost>(this, "RemoveHost", (obj, host) =>
            {
                //_hosts.Remove(host);
            });
        }

        async System.Threading.Tasks.Task ExecuteRefreshHostsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await App.Manager.Update();
            IsBusy = false;
        }

        private async System.Threading.Tasks.Task ExecuteAddHostsCommandAsync()
        {
            await _navigation.PushAsync(new AddHostPage());
        }
    }
}