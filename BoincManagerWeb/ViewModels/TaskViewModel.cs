using System.Linq;
using BoincManager;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWeb.ViewModels
{
    public class TaskViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string Application { get; }
        public string Workunit { get; }
        public double Progress { get; }
        public string ElapsedTime { get; }
        public string CpuTime { get; }
        public string CpuTimeRemaining { get; }
        public string LastCheckpoint { get; }
        public string Deadline { get; }
        public string Status { get; }

        public Project RpcProject { get; }
        public Workunit RpcWorkunit { get; }
        public App RpcApp { get; }
        public AppVersion RpcAppVersion { get; }

        public TaskViewModel(HostState hostState, Result result)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            RpcProject = hostState.BoincState.Projects?.FirstOrDefault(p => p.MasterUrl == result.ProjectUrl);
            RpcWorkunit = hostState.BoincState.Workunits?.FirstOrDefault(w => w.ProjectUrl == RpcProject?.MasterUrl && w.Name == result.WorkunitName);
            RpcApp = hostState.BoincState.Apps?.FirstOrDefault(a => a.ProjectUrl == RpcProject?.MasterUrl && a.Name == RpcWorkunit?.AppName);

            //RpcAppVersion = result.VersionNumber != 0
            //    ? boincState.AppVersions?.FirstOrDefault(av => av.ProjectUrl == result.ProjectUrl && av.AppName == RpcApp?.Name && av.VersionNumber == result.VersionNumber && av.PlanClass == result.PlanClass)
            //    : boincState.AppVersions?.FirstOrDefault(av => av.ProjectUrl == result.ProjectUrl && av.AppName == RpcApp?.Name && av.VersionNumber == RpcWorkunit?.VersionNumber);

            if (RpcProject == null || RpcWorkunit == null || RpcApp == null/* || RpcAppVersion == null*/)
            {
                Status = Statuses.GetTaskStatus(result, RpcProject, hostState.BoincState);
                return;
            }

            Project = RpcProject.ProjectName;
            Application = RpcApp.UserFriendlyName;
            Workunit = result.WorkunitName;
            Progress = result.ReadyToReport ? 1.0 : result.FractionDone;
            ElapsedTime = Utils.ConvertDuration(result.ElapsedTime);
            CpuTime = Utils.ConvertDuration(result.CurrentCpuTime);
            CpuTimeRemaining = Utils.ConvertDuration(result.EstimatedCpuTimeRemaining);
            LastCheckpoint = Utils.ConvertDuration(result.CurrentCpuTime - result.CheckpointCpuTime);
            Deadline = Utils.ConvertDateTime(result.ReportDeadline);
            Status = Statuses.GetTaskStatus(result, RpcProject, hostState.BoincState);
        }        
    }
}
