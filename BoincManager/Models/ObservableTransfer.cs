using BoincManager;
using BoincManager.Interfaces;
using BoincManager.Models;
using System.Collections.Generic;

namespace BoincManager.Models
{
    public class ObservableTransfer : BindableBase, ITransfer, IFilterable
    {
        public int HostId { get; }
        public string HostName { get; }

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
        public string FileSize
        {
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

        public BoincRpc.FileTransfer FileTransfer { get; private set; }

        public ObservableTransfer(HostState hostState, BoincRpc.FileTransfer fileTransfer)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            Update(fileTransfer);
        }

        public void Update(BoincRpc.FileTransfer fileTransfer)
        {
            FileTransfer = fileTransfer;

            Project = fileTransfer.ProjectName;
            FileName = fileTransfer.Name;
            Progress = fileTransfer.NumberOfBytes > 0 ? fileTransfer.BytesTransferred / fileTransfer.NumberOfBytes : 0;
            FileSize = Utils.ConvertBytesToFileSize(fileTransfer.NumberOfBytes);
            TransferRate = fileTransfer.TransferActive ? $"{Utils.ConvertBytesToFileSize(fileTransfer.TransferSpeed)} /s" : string.Empty;
            ElapsedTime = Utils.ConvertDuration(fileTransfer.TimeSoFar);
            TimeRemaining = fileTransfer.TransferActive ? Utils.GetTimeRemaining(fileTransfer) : string.Empty;
            Status = Statuses.GetTransferStatus(fileTransfer);
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
    }
}
