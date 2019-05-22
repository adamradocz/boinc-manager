using System.Collections.Generic;
using BoincManager.Models;

namespace BoincManagerWindows.ViewModels
{
    class TaskViewModel : BoincManager.ViewModels.TaskViewModel, IFilterableViewModel
    {
        string project;
        public override string Project
        {
            get { return project; }
            protected set { SetProperty(ref project, value); }
        }

        string application;
        public override string Application
        {
            get { return application; }
            protected set { SetProperty(ref application, value); }
        }

        string workunit;
        public override string Workunit
        {
            get { return workunit; }
            protected set { SetProperty(ref workunit, value); }
        }

        double progress;
        public override double Progress
        {
            get { return progress; }
            protected set { SetProperty(ref progress, value); }
        }

        string elapsedTime;
        public override string ElapsedTime
        {
            get { return elapsedTime; }
            protected set { SetProperty(ref elapsedTime, value); }
        }

        string cpuTime;
        public override string CpuTime
        {
            get { return cpuTime; }
            protected set { SetProperty(ref cpuTime, value); }
        }

        string cpuTimeRemaining;
        public override string CpuTimeRemaining
        {
            get { return cpuTimeRemaining; }
            protected set { SetProperty(ref cpuTimeRemaining, value); }
        }

        string lastCheckpoint;
        public override string LastCheckpoint
        {
            get { return lastCheckpoint; }
            protected set { SetProperty(ref lastCheckpoint, value); }
        }

        string deadline;
        public override string Deadline
        {
            get { return deadline; }
            protected set { SetProperty(ref deadline, value); }
        }

        string status;
        public override string Status
        {
            get { return status; }
            protected set { SetProperty(ref status, value); }
        }      

        public TaskViewModel(HostState hostState) : base(hostState)
        {
        }
        
        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
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
            yield return nameof(HostName);
            yield return nameof(Project);
            //yield return nameof(CpuTime);
            //yield return nameof(CpuTimeRemaining);
            //yield return nameof(LastCheckpoint);
            //yield return nameof(Deadline);
            yield return nameof(Status);
        }
    }
}
