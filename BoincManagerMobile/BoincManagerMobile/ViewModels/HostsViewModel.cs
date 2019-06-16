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
        public ObservableCollection<Host> Hosts { get; set; }
        public Command LoadItemsCommand { get; set; }

        public HostsViewModel()
        {
            Title = nameof(MenuItemType.Hosts);
            Hosts = GetHosts(App.Manager.GetAllHostStates(), "");

            LoadItemsCommand = new Command(() => ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<AddHostPage, Host>(this, "AddHost", (obj, hostViewModel) =>
            {
                Hosts.Add(hostViewModel);
            });
        }

        public ObservableCollection<Host> GetHosts(IEnumerable<HostState> hostStates, string searchString)
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

        void ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Hosts.Clear();
                var items = GetHosts(App.Manager.GetAllHostStates(), "");
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
    }
}