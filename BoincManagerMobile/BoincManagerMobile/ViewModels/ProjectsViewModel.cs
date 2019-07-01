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
    public class ProjectsViewModel : BaseViewModel
    {
        private readonly INavigation _navigation;

        private ObservableCollection<Project> _projects;
        public ObservableCollection<Project> Projects { get => _projects; }
        public Command LoadProjectsCommand { get; }
        public Command AddProjectsCommand { get; }

        public ProjectsViewModel(INavigation navigation)
        {
            Title = nameof(MenuItemType.Projects);

            _navigation = navigation;

            _projects = new ObservableCollection<Project>();

            LoadProjectsCommand = new Command(() => ExecuteLoadProjectsCommand());
            AddProjectsCommand = new Command(async () => await ExecuteAddProjectsCommand());
        }

        void ExecuteLoadProjectsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _projects.Clear();
                GetProjects(App.Manager.GetAllHostStates(), string.Empty, ref _projects);                
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

        public void GetProjects(IEnumerable<HostState> hostStates, string searchString, ref ObservableCollection<Project> projects)
        {
            foreach (var hostState in hostStates)
            {
                if (hostState.Connected)
                {
                    foreach (var rpcProject in hostState.BoincState.Projects)
                    {
                        if (string.IsNullOrEmpty(searchString))
                        {
                            projects.Add(new Project(hostState, rpcProject));
                        }
                        else
                        {
                            var project = new Project(hostState, rpcProject);
                            foreach (var content in project.GetContentsForFiltering())
                            {
                                if (content != null && content.IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase) != -1)
                                {
                                    // The search string is found in any of the VM's property
                                    projects.Add(project);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private async System.Threading.Tasks.Task ExecuteAddProjectsCommand()
        {
            await _navigation.PushAsync(new AddProjectPage());
        }
    }
}