using BoincManager;
using BoincManager.Interfaces;
using BoincManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoincManagerMobile.Models
{
    public class Task : BindableBase, ITask, IFilterable
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

        public Task(HostState hostState, BoincRpc.Result result)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            var rpcProject = hostState.BoincState.Projects?.FirstOrDefault(p => p.MasterUrl == result.ProjectUrl);
            var rpcWorkunit = hostState.BoincState.Workunits?.FirstOrDefault(w => w.ProjectUrl == rpcProject?.MasterUrl && w.Name == result.WorkunitName);
            var rpcApp = hostState.BoincState.Apps?.FirstOrDefault(a => a.ProjectUrl == rpcProject?.MasterUrl && a.Name == rpcWorkunit?.AppName);

            if (rpcProject == null || rpcWorkunit == null || rpcApp == null)
            {
                Status = Statuses.GetTaskStatus(result, rpcProject, hostState.BoincState);
                return;
            }

            Project = rpcProject.ProjectName;
            Application = rpcApp.UserFriendlyName;
            Workunit = result.WorkunitName;
            Progress = result.ReadyToReport ? 1 : result.FractionDone;
            ElapsedTime = BoincManager.Utils.ConvertDuration(result.ElapsedTime);
            CpuTime = BoincManager.Utils.ConvertDuration(result.CurrentCpuTime);
            CpuTimeRemaining = BoincManager.Utils.ConvertDuration(result.EstimatedCpuTimeRemaining);
            LastCheckpoint = BoincManager.Utils.ConvertDuration(result.CurrentCpuTime - result.CheckpointCpuTime);
            Deadline = BoincManager.Utils.ConvertDateTime(result.ReportDeadline);
            Status = Statuses.GetTaskStatus(result, rpcProject, hostState.BoincState);
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
    }
}
