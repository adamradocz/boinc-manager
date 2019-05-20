using BoincManager.Models;
using BoincRpc;

namespace BoincManager.ViewModels
{
    public class ProjectViewModel
    {
        public int HostId { get; }
        public string HostName { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Team { get; set; }
        public string Credit { get; set; }
        public string AverageCredit { get; set; }
        public string Status { get; set; }
        public Project Project { get; private set; }

        public ProjectViewModel(HostState hostState)
        {
            HostId = hostState.Id;
            HostName = hostState.HostName;
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
