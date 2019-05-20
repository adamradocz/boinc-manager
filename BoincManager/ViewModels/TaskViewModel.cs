using System.Linq;
using BoincManager.Models;
using BoincRpc;

namespace BoincManager.ViewModels
{
    public class TaskViewModel
    {
        public int HostId { get; }
        public string HostName { get; private set; }
        public string Project { get; set; }
        public string Application { get; set; }
        public string Workunit { get; set; }
        public double Progress { get; set; }
        public string ElapsedTime { get; set; }
        public string CpuTime { get; set; }
        public string CpuTimeRemaining { get; set; }
        public string LastCheckpoint { get; set; }
        public string Deadline { get; set; }
        public string Status { get; set; }

        public Result RpcResult { get; private set; }
        public Project RpcProject { get; private set; }
        public Workunit RpcWorkunit { get; private set; }
        public BoincRpc.App RpcApp { get; private set; }
        public AppVersion RpcAppVersion { get; private set; }

        public TaskViewModel(int hostId, string hostName)
        {
            HostId = hostId;
            HostName = hostName;
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
