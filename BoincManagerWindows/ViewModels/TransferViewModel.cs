using System.Collections.Generic;
using BoincRpc;

namespace BoincManagerWindows.ViewModels
{
    class TransferViewModel : ViewModel, IFilterableViewModel
    {
        public string ComputerId { get; }
        public string ComputerName { get; private set; }

        string project;
        public string Project
        {
            get { return project; }
            private set { SetProperty(ref project, value); }
        }

        string fileName;
        public string FileName
        {
            get { return fileName; }
            private set { SetProperty(ref fileName, value); }
        }

        double progress;
        public double Progress
        {
            get { return progress; }
            private set { SetProperty(ref progress, value); }
        }

        string fileSize;
        public string FileSize {
            get { return fileSize; }
            private set { SetProperty(ref fileSize, value); }
        }

        string transferRate;
        public string TransferRate
        {
            get { return transferRate; }
            private set { SetProperty(ref transferRate, value); }
        }

        string elapsedTime;
        public string ElapsedTime
        {
            get { return elapsedTime; }
            private set { SetProperty(ref elapsedTime, value); }
        }

        string timeRemaining;
        public string TimeRemaining
        {
            get { return timeRemaining; }
            private set { SetProperty(ref timeRemaining, value); }
        }

        string status;
        public string Status
        {
            get { return status; }
            private set { SetProperty(ref status, value); }
        }

        public FileTransfer FileTransfer { get; private set; }

        public TransferViewModel(string computerId, string computerName)
        {
            ComputerId = computerId;
            ComputerName = computerName;
        }

        public void Update(FileTransfer fileTransfer)
        {
            FileTransfer = fileTransfer;

            Project = fileTransfer.ProjectName;
            FileName = fileTransfer.Name;
            Progress = FileTransfer.NumberOfBytes > 0 ? fileTransfer.BytesTransferred / FileTransfer.NumberOfBytes : 0;
            FileSize = BoincManager.Utils.ConvertBytesToFileSize(fileTransfer.NumberOfBytes);
            TransferRate = FileTransfer.TransferActive ? $"{BoincManager.Utils.ConvertBytesToFileSize(fileTransfer.TransferSpeed)} /s" : null;
            ElapsedTime = BoincManager.Utils.ConvertDuration(fileTransfer.TimeSoFar);
            TimeRemaining = FileTransfer.TransferActive ? BoincManager.Utils.GetTimeRemaining(fileTransfer) : null;
            Status = BoincManager.Statuses.GetTransferStatus(fileTransfer);
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
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
            //yield return nameof(TransferRate);
            //yield return nameof(ElapsedTime);
            //yield return nameof(TimeRemaining);
            yield return nameof(Status);
        }
    }
}
