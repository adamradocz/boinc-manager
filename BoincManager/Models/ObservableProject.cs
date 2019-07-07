using BoincManager.Interfaces;
using System;
using System.Collections.Generic;

namespace BoincManager.Models
{
    public class ObservableProject : BindableBase, IProject, IFilterable, IEquatable<ObservableProject>
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

        public ObservableProject(HostState hostState, BoincRpc.Project rpcProject)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            Update(rpcProject);
        }

        public void Update(BoincRpc.Project rpcProject)
        {
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


        #region Equality comparisons
        /* From:
         * - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
         * - https://intellitect.com/overidingobjectusingtuple/
         * - https://montemagno.com/optimizing-c-struct-equality-with-iequatable/
        */

        public bool Equals(ObservableProject other)
        {
            // If parameter is null, return false.
            if (other is null)
                return false;

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
                return true;

            return HostId == other.HostId
                && Name.Equals(other.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            ObservableProject project = obj as ObservableProject;
            return project == null ? false : Equals(project);
        }

        public override int GetHashCode()
        {
            //return HashCode.Combine(HostId, RpcResult.Name); Available in .NET Strandard 2.1, but the current Xamarin version doesn't support it, only .NET Standard 2.0
            return (HostId, Name).GetHashCode();
        }

        public static bool operator ==(ObservableProject lhs, ObservableProject rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ObservableProject lhs, ObservableProject rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
