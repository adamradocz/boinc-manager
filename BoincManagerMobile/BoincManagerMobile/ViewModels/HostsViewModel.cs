using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xamarin.Forms;
using System.Collections.Generic;
using BoincManager.Models;
using BoincManagerMobile.Models;
using BoincManagerMobile.Views;

namespace BoincManagerMobile.ViewModels
{
    public class HostsViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        private ObservableCollection<Host> _hosts;
        public ObservableCollection<Host> Hosts { get => _hosts; set => _hosts = value; }
        public Command LoadHostsCommand { get; }
        public Command AddHostsCommand { get; }

        public HostsViewModel(INavigation navigation)
        {
            Title = nameof(MenuItemType.Hosts);

            _navigation = navigation;

            Hosts = new ObservableCollection<Host>();

            LoadHostsCommand = new Command(() => ExecuteLoadHostsCommand());
            AddHostsCommand = new Command(async () => await ExecuteAddHostsCommandAsync());

            MessagingCenter.Subscribe<AddHostPage, Host>(this, "AddHost", (obj, host) =>
            {
                Hosts.Add(host);
            });

            MessagingCenter.Subscribe<HostDetailPage, Host>(this, "RemoveHost", (obj, host) =>
            {
                Hosts.Remove(host);
            });
        }

        void ExecuteLoadHostsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Hosts.Clear();
                GetHosts(App.Manager.GetAllHostStates(), string.Empty, ref _hosts);                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void GetHosts(IEnumerable<HostState> hostStates, string searchString, ref ObservableCollection<Host> hosts)
        {
            foreach (var hostState in hostStates)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    hosts.Add(new Host(hostState));
                }
                else
                {
                    var hostVM = new Host(hostState);
                    foreach (var content in hostVM.GetContentsForFiltering())
                    {
                        if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            // The search string is found in any of the VM's property
                            hosts.Add(hostVM);
                            break;
                        }
                    }
                }
            }
        }

        private async System.Threading.Tasks.Task ExecuteAddHostsCommandAsync()
        {
            await _navigation.PushAsync(new AddHostPage());
        }
    }
}