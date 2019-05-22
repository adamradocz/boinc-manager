using System.Collections.Generic;
using BoincManager.Models;

namespace BoincManagerWindows.ViewModels
{
    class TransferViewModel : BoincManager.ViewModels.TransferViewModel, IFilterableViewModel
    {
        string project;
        public override string Project
        {
            get { return project; }
            protected set { SetProperty(ref project, value); }
        }

        string fileName;
        public override string FileName
        {
            get { return fileName; }
            protected set { SetProperty(ref fileName, value); }
        }

        double progress;
        public override double Progress
        {
            get { return progress; }
            protected set { SetProperty(ref progress, value); }
        }

        string fileSize;
        public override string FileSize {
            get { return fileSize; }
            protected set { SetProperty(ref fileSize, value); }
        }

        string transferRate;
        public override string TransferRate
        {
            get { return transferRate; }
            protected set { SetProperty(ref transferRate, value); }
        }

        string elapsedTime;
        public override string ElapsedTime
        {
            get { return elapsedTime; }
            protected set { SetProperty(ref elapsedTime, value); }
        }

        string timeRemaining;
        public override string TimeRemaining
        {
            get { return timeRemaining; }
            protected set { SetProperty(ref timeRemaining, value); }
        }

        string status;
        public override string Status
        {
            get { return status; }
            protected set { SetProperty(ref status, value); }
        }

        public TransferViewModel(HostState hostState) : base(hostState)
        {
        }
        
        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
            yield return Project;
            yield return FileName;
            yield return FileSize;
            yield return TransferRate;
            yield return ElapsedTime;
            yield return TimeRemaining;
            yield return Status;
        }

        public static IEnumerable<string> GetLiveFilteringProperties()
        {
            yield return nameof(HostName);
            yield return nameof(Project);
            //yield return nameof(TransferRate);
            //yield return nameof(ElapsedTime);
            //yield return nameof(TimeRemaining);
            yield return nameof(Status);
        }
    }
}
