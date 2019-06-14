using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using BoincManagerMobile;
using BoincManagerMobile.Views;
using System.Collections.Generic;
using BoincManager.Models;

namespace BoincManagerMobile.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<HostViewModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Projects";
            Items = new ObservableCollection<HostViewModel>();
                        
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            /*MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                Items.Add(newItem);
                await DataStore.AddItemAsync(newItem);
            });*/
            Items = GetHosts(App._manager.GetAllHostStates(), "");
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

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                //var items = await DataStore.GetItemsAsync(true);
                var items = GetHosts(App._manager.GetAllHostStates(), "");
                foreach (var item in items)
                {
                    Items.Add(item);
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