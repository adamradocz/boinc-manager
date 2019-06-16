using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Xamarin.Forms;
using System.Collections.Generic;
using BoincManager.Models;
using BoincManagerMobile.Models;

namespace BoincManagerMobile.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        public ObservableCollection<Task> Tasks { get; set; }
        public Command LoadItemsCommand { get; set; }

        public TasksViewModel()
        {
            Title = nameof(MenuItemType.Hosts);
            Tasks = GetTasks(App.Manager.GetAllHostStates(), "");

            LoadItemsCommand = new Command(() => ExecuteLoadItemsCommand());
        }

        public ObservableCollection<Task> GetTasks(IEnumerable<HostState> hostStates, string searchString)
        {
            var tasks = new ObservableCollection<Task>();
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcResult in hostState.BoincState.Results)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            tasks.Add(new Models.Task(hostState, rpcResult));
                        }
                        else
                        {
                            var task = new Models.Task(hostState, rpcResult);
                            foreach (var content in task.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    tasks.Add(task);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return tasks;
        }

        void ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Tasks.Clear();
                var items = GetTasks(App.Manager.GetAllHostStates(), "");
                foreach (var item in items)
                {
                    Tasks.Add(item);
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