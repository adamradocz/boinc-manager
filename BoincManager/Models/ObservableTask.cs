using BoincManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoincManager.Models
{
    public class ObservableTask : BindableBase, ITask, IFilterable, IEquatable<ObservableTask>
    {
        public int HostId { get; }
        public string HostName { get; }

        string project;
        public string Project
        {
            get { return project; }
            private set { SetProperty(ref project, value); }
        }

        string application;
        public string Application
        {
            get { return application; }
            private set { SetProperty(ref application, value); }
        }

        string workunit;
        public string Workunit
        {
            get { return workunit; }
            private set { SetProperty(ref workunit, value); }
        }

        double progress;
        public double Progress
        {
            get { return progress; }
            private set { SetProperty(ref progress, value); }
        }

        string elapsedTime;
        public string ElapsedTime
        {
            get { return elapsedTime; }
            private set { SetProperty(ref elapsedTime, value); }
        }

        string cpuTime;
        public string CpuTime
        {
            get { return cpuTime; }
            private set { SetProperty(ref cpuTime, value); }
        }

        string cpuTimeRemaining;
        public string CpuTimeRemaining
        {
            get { return cpuTimeRemaining; }
            private set { SetProperty(ref cpuTimeRemaining, value); }
        }

        string lastCheckpoint;
        public string LastCheckpoint
        {
            get { return lastCheckpoint; }
            private set { SetProperty(ref lastCheckpoint, value); }
        }

        string deadline;
        public string Deadline
        {
            get { return deadline; }
            private set { SetProperty(ref deadline, value); }
        }

        string status;
        public string Status
        {
            get { return status; }
            private set { SetProperty(ref status, value); }
        }

        public BoincRpc.Result RpcResult { get; private set; }
        public BoincRpc.Project RpcProject { get; }

        public ObservableTask(HostState hostState, BoincRpc.Result result)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            RpcProject = hostState.BoincState.Projects?.FirstOrDefault(p => p.MasterUrl == result.ProjectUrl);
            var rpcWorkunit = hostState.BoincState.Workunits?.FirstOrDefault(w => w.ProjectUrl == RpcProject?.MasterUrl && w.Name == result.WorkunitName);
            var rpcApp = hostState.BoincState.Apps?.FirstOrDefault(a => a.ProjectUrl == RpcProject?.MasterUrl && a.Name == rpcWorkunit?.AppName);

            if (RpcProject == null || rpcWorkunit == null || rpcApp == null)
            {
                Status = Statuses.GetTaskStatus(result, RpcProject, hostState.BoincState);
                return;
            }

            Project = RpcProject.ProjectName;
            Application = rpcApp.UserFriendlyName;
            Workunit = result.WorkunitName;

            Update(hostState, result);
        }

        public void Update(HostState hostState, BoincRpc.Result result)
        {
            RpcResult = result;

            Progress = result.ReadyToReport ? 1 : result.FractionDone;
            ElapsedTime = Utils.ConvertDuration(result.ElapsedTime);
            CpuTime = Utils.ConvertDuration(result.CurrentCpuTime);
            CpuTimeRemaining = Utils.ConvertDuration(result.EstimatedCpuTimeRemaining);
            LastCheckpoint = Utils.ConvertDuration(result.CurrentCpuTime - result.CheckpointCpuTime);
            Deadline = Utils.ConvertDateTime(result.ReportDeadline);
            Status = Statuses.GetTaskStatus(result, RpcProject, hostState.BoincState);
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
            yield return Project;
            yield return Application;
            yield return Workunit;
            yield return CpuTime;
            yield return CpuTimeRemaining;
            yield return LastCheckpoint;
            yield return Deadline;
            yield return Status;
        }

        public static IEnumerable<string> GetLiveFilteringProperties()
        {
            yield return nameof(Workunit);
            yield return nameof(CpuTime);
            yield return nameof(CpuTimeRemaining);
            yield return nameof(LastCheckpoint);
            yield return nameof(Deadline);
            yield return nameof(Status);
        }

        #region Equality comparisons
        /* From:
         * - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
         * - https://intellitect.com/overidingobjectusingtuple/
         * - https://montemagno.com/optimizing-c-struct-equality-with-iequatable/
        */

        public bool Equals(ObservableTask other)
        {
            // If parameter is null, return false.
            if (other is null)
                return false;

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
                return true;

            return HostId == other.HostId
                && RpcResult.Name.Equals(other.RpcResult.Name, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            ObservableTask task = obj as ObservableTask;
            return task == null ? false : Equals(task);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HostId, Project, RpcResult.Name);
        }

        public static bool operator ==(ObservableTask lhs, ObservableTask rhs)
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

        public static bool operator !=(ObservableTask lhs, ObservableTask rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
