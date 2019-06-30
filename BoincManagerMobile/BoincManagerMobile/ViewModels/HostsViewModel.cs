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

        public ObservableCollection<Host> Hosts { get; set; }
        public Command LoadHostsCommand { get; set; }
        public Command AddHostsCommand { get; set; }

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
                var items = GetHosts(App.Manager.GetAllHostStates(), string.Empty);
                foreach (var item in items)
                {
                    Hosts.Add(item);
                }
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

        private Collection<Host> GetHosts(IEnumerable<HostState> hostStates, string searchString)
        {
            var hosts = new ObservableCollection<Host>();
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

            return hosts;
        }

        private async System.Threading.Tasks.Task ExecuteAddHostsCommandAsync()
        {
            await _navigation.PushAsync(new AddHostPage());
        }
    }
}