using System.Collections.Generic;
using BoincManager;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class ProjectViewModel : BindableBase, IFilterableViewModel
    {
        public int HostId { get; }
        public string HostName { get; }

        string name;
        public string Name
        {
            get { return name; }
            private set { SetProperty(ref name, value); }
        }

        string username;
        public string Username
        {
            get { return username; }
            private set { SetProperty(ref username, value); }
        }

        string team;
        public string Team
        {
            get { return team; }
            private set { SetProperty(ref team, value); }
        }

        string credit;
        public string Credit
        {
            get { return credit; }
            private set { SetProperty(ref credit, value); }
        }

        string averageCredit;
        public string AverageCredit
        {
            get { return averageCredit; }
            private set { SetProperty(ref averageCredit, value); }
        }       

        string status;
        public string Status
        {
            get { return status; }
            private set { SetProperty(ref status, value); }
        }

        public Project Project { get; private set; }

        public ProjectViewModel(HostState hostState)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;
        }

        public void Update(Project project)
        {
            Project = project;

            Name = project.ProjectName;
            Username = project.UserName;
            Team = project.TeamName;
            Credit = project.UserTotalCredit.ToString("N2");
            AverageCredit = project.UserAverageCredit.ToString("N2");
            Status = Statuses.GetProjectStatus(project);
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
            yield return Name;
            yield return Username;
            yield return Team;
            yield return Credit;
            yield return AverageCredit;
            yield return Status;
        }

        public static IEnumerable<string> GetLiveFilteringProperties()
        {
            yield return nameof(HostName);
            yield return nameof(Name);
            yield return nameof(Username);
            yield return nameof(Team);
            yield return nameof(Status);
        }
    }
}
