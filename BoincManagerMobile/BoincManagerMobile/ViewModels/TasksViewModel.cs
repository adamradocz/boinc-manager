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
        private ObservableCollection<Task> _tasks;
        public ObservableCollection<Task> Tasks { get => _tasks; set => _tasks = value; }
        public Command LoadTasksCommand { get; }

        public TasksViewModel()
        {
            Title = nameof(MenuItemType.Tasks);
            Tasks = new ObservableCollection<Task>();

            LoadTasksCommand = new Command(() => ExecuteLoadTasksCommand());
        }

        void ExecuteLoadTasksCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Tasks.Clear();
                GetTasks(App.Manager.GetAllHostStates(), string.Empty, ref _tasks);
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

        private void GetTasks(IEnumerable<HostState> hostStates, string searchString, ref ObservableCollection<Task> tasks)
        {
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
                                    // The search string is found in any of the Models's property
                                    tasks.Add(task);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}