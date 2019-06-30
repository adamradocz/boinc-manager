using BoincManager;
using BoincManager.Interfaces;
using BoincManager.Models;
using System.Collections.Generic;

namespace BoincManagerMobile.Models
{
    public class Project : BindableBase, IProject, IFilterable
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

        public BoincRpc.Project RpcProject { get; private set; }

        public Project(HostState hostState, BoincRpc.Project rpcProject)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            RpcProject = rpcProject;
            Name = rpcProject.ProjectName;
            Username = rpcProject.UserName;
            Team = rpcProject.TeamName;
            Credit = rpcProject.UserTotalCredit.ToString("N2");
            AverageCredit = rpcProject.UserAverageCredit.ToString("N2");
            Status = Statuses.GetProjectStatus(rpcProject);
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
    }
}
