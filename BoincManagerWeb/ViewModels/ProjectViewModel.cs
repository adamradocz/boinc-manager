using BoincManager;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWeb.ViewModels
{
    public class ProjectViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Name { get; }
        public string Username { get; }
        public string Team { get; }
        public string Credit { get; }
        public string AverageCredit { get; }
        public string Status { get; }

        public ProjectViewModel(HostState hostState, Project project)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            Name = project.ProjectName;
            Username = project.UserName;
            Team = project.TeamName;
            Credit = project.UserTotalCredit.ToString("N2");
            AverageCredit = project.UserAverageCredit.ToString("N2");
            Status = Statuses.GetProjectStatus(project);
        }
    }
}
