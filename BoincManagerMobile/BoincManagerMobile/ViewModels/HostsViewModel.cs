using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using BoincManagerMobile;
using BoincManagerMobile.Views;
using System.Collections.Generic;
using BoincManager.Models;
using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class HostsViewModel : BaseViewModel
    {
        public ObservableCollection<HostViewModel> Hosts { get; set; }
        public Command LoadItemsCommand { get; set; }

        public HostsViewModel()
        {
            Title = nameof(MenuItemType.Hosts);
            Hosts = GetHosts(App.Manager.GetAllHostStates(), "");

            LoadItemsCommand = new Command(() => ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<AddHostPage, HostViewModel>(this, "AddHost", (obj, hostViewModel) =>
            {
                Hosts.Add(hostViewModel);
            });
        }

        public ObservableCollection<HostViewModel> GetHosts(IEnumerable<HostState> hostStates, string searchString)
        {
            var hosts = new ObservableCollection<HostViewModel>();
            foreach (var hostState in hostStates)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    hosts.Add(new HostViewModel(hostState));
                }
                else
                {
                    var hostVM = new HostViewModel(hostState);
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

        public ObservableCollection<ProjectViewModel> GetProjects(IEnumerable<HostState> hostStates, string searchString)
        {
            var projects = new ObservableCollection<ProjectViewModel>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var project in hostState.BoincState.Projects)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            projects.Add(new ProjectViewModel(hostState, project));
                        }
                        else
                        {
                            var projectVM = new ProjectViewModel(hostState, project);
                            foreach (var content in projectVM.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    projects.Add(projectVM);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return projects;
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