using System.Collections.Generic;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class ProjectViewModel : ViewModel, IFilterableViewModel
    {
        public string ComputerId { get; }
        public string ComputerName { get; private set; }

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

        public ProjectViewModel(string computerId, string computerName)
        {
            ComputerId = computerId;
            ComputerName = computerName;
        }

        public void Update(Project project)
        {
            Project = project;

            Name = project.ProjectName;
            Username = project.UserName;
            Team = project.TeamName;
            Credit = project.UserTotalCredit.ToString("N2");
            AverageCredit = project.UserAverageCredit.ToString("N2");
            Status = BoincManager.Statuses.GetProjectStatus(project);
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return Name;
            yield return Username;
            yield return Team;
            yield return Credit;
            yield return AverageCredit;
            yield return Status;
        }

        public static IEnumerable<string> GetLiveFilteringProperties()
        {
            yield return nameof(Status);
        }
    }
}
