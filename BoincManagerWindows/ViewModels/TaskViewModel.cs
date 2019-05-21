using System.Collections.Generic;
using System.Linq;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class TaskViewModel : BaseViewModel, IFilterableViewModel
    {
        public string ComputerId { get; }
        public string ComputerName { get; private set; }
        
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

        public Result RpcResult { get; private set; }
        public Project RpcProject { get; private set; }
        public Workunit RpcWorkunit { get; private set; }
        public BoincRpc.App RpcApp { get; private set; }
        public AppVersion RpcAppVersion { get; private set; }

        public TaskViewModel(string computerId, string computerName)
        {
            ComputerId = computerId;
            ComputerName = computerName;
        }
        
        public void Update(Result result, BoincState boincState)
        {
            RpcProject = boincState.Projects?.FirstOrDefault(p => p.MasterUrl == result.ProjectUrl);            
            RpcWorkunit = boincState.Workunits?.FirstOrDefault(w => w.ProjectUrl == RpcProject?.MasterUrl && w.Name == result.WorkunitName);
            RpcApp = boincState.Apps?.FirstOrDefault(a => a.ProjectUrl == RpcProject?.MasterUrl && a.Name == RpcWorkunit?.AppName);

            //RpcAppVersion = result.VersionNumber != 0
            //    ? boincState.AppVersions?.FirstOrDefault(av => av.ProjectUrl == result.ProjectUrl && av.AppName == RpcApp?.Name && av.VersionNumber == result.VersionNumber && av.PlanClass == result.PlanClass)
            //    : boincState.AppVersions?.FirstOrDefault(av => av.ProjectUrl == result.ProjectUrl && av.AppName == RpcApp?.Name && av.VersionNumber == RpcWorkunit?.VersionNumber);

            if (RpcProject == null || RpcWorkunit == null || RpcApp == null/* || RpcAppVersion == null*/)
            {
                Status = BoincManager.Statuses.GetTaskStatus(result, RpcProject, boincState);
                return;
            }

            RpcResult = result;
            
            Project = RpcProject.ProjectName;
            Application = RpcApp.UserFriendlyName;
            Workunit = result.WorkunitName;
            Progress = result.ReadyToReport ? 1.0 : result.FractionDone;
            ElapsedTime = BoincManager.Utils.ConvertDuration(result.ElapsedTime);
            CpuTime = BoincManager.Utils.ConvertDuration(result.CurrentCpuTime);
            CpuTimeRemaining = BoincManager.Utils.ConvertDuration(result.EstimatedCpuTimeRemaining);
            LastCheckpoint = BoincManager.Utils.ConvertDuration(result.CurrentCpuTime - result.CheckpointCpuTime);
            Deadline = BoincManager.Utils.ConvertDateTime(result.ReportDeadline);
            Status = BoincManager.Statuses.GetTaskStatus(result, RpcProject, boincState);
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return ComputerName;
            yield return Project;
            yield return Application;
            yield return Workunit;
            //yield return Progress;
            yield return CpuTime;
            yield return CpuTimeRemaining;
            yield return LastCheckpoint;
            yield return Deadline;
            yield return Status;
        }

        public static IEnumerable<string> GetLiveFilteringProperties()
        {
            yield return nameof(ComputerName);
            yield return nameof(Project);
            //yield return nameof(CpuTime);
            //yield return nameof(CpuTimeRemaining);
            //yield return nameof(LastCheckpoint);
            //yield return nameof(Deadline);
            yield return nameof(Status);
        }
    }
}
