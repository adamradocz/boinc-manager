﻿using System;
using System.Collections.Generic;
using System.Linq;
using BoincManager;
using BoincManager.Interfaces;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWeb.Models
{
    public class Task : ITask, IFilterable
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

        public Task(HostState hostState, Result result)
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
            Progress = result.ReadyToReport ? 100 : Math.Round(result.FractionDone * 100, 3);
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
