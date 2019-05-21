using System.Linq;
using BoincManager.Models;
using BoincRpc;

namespace BoincManager.ViewModels
{
    public class TaskViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; private set; }
        public string Application { get; private set; }
        public string Workunit { get; private set; }
        public double Progress { get; private set; }
        public string ElapsedTime { get; private set; }
        public string CpuTime { get; private set; }
        public string CpuTimeRemaining { get; private set; }
        public string LastCheckpoint { get; private set; }
        public string Deadline { get; private set; }
        public string Status { get; private set; }

        public Result RpcResult { get; private set; }
        public Project RpcProject { get; private set; }
        public Workunit RpcWorkunit { get; private set; }
        public BoincRpc.App RpcApp { get; private set; }
        public AppVersion RpcAppVersion { get; private set; }

        public TaskViewModel(HostState hostState)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;
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
                Status = Statuses.GetTaskStatus(result, RpcProject, boincState);
                return;
            }

            RpcResult = result;
            
            Project = RpcProject.ProjectName;
            Application = RpcApp.UserFriendlyName;
            Workunit = result.WorkunitName;
            Progress = result.ReadyToReport ? 1.0 : result.FractionDone;
            ElapsedTime = Utils.ConvertDuration(result.ElapsedTime);
            CpuTime = Utils.ConvertDuration(result.CurrentCpuTime);
            CpuTimeRemaining = Utils.ConvertDuration(result.EstimatedCpuTimeRemaining);
            LastCheckpoint = Utils.ConvertDuration(result.CurrentCpuTime - result.CheckpointCpuTime);
            Deadline = Utils.ConvertDateTime(result.ReportDeadline);
            Status = Statuses.GetTaskStatus(result, RpcProject, boincState);
        }
    }
}
