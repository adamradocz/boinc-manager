using BoincManager.Models;
using BoincRpc;

namespace BoincManager.ViewModels
{
    public class ProjectViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Name { get; private set; }
        public string Username { get; private set; }
        public string Team { get; private set; }
        public string Credit { get; private set; }
        public string AverageCredit { get; private set; }
        public string Status { get; private set; }

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
    }
}
