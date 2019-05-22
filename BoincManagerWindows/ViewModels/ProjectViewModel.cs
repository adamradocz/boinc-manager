using System.Collections.Generic;
using BoincManager.Models;

namespace BoincManagerWindows.ViewModels
{
    class ProjectViewModel : BoincManager.ViewModels.ProjectViewModel, IFilterableViewModel
    {
        string name;
        public override string Name
        {
            get { return name; }
            protected set { SetProperty(ref name, value); }
        }

        string username;
        public override string Username
        {
            get { return username; }
            protected set { SetProperty(ref username, value); }
        }

        string team;
        public override string Team
        {
            get { return team; }
            protected set { SetProperty(ref team, value); }
        }

        string credit;
        public override string Credit
        {
            get { return credit; }
            protected set { SetProperty(ref credit, value); }
        }

        string averageCredit;
        public override string AverageCredit
        {
            get { return averageCredit; }
            protected set { SetProperty(ref averageCredit, value); }
        }       

        string status;
        public override string Status
        {
            get { return status; }
            protected set { SetProperty(ref status, value); }
        }

        public ProjectViewModel(HostState hostState) : base(hostState)
        {
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
