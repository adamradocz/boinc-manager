using BoincManager.Models;
using BoincRpc;

namespace BoincManager.ViewModels
{
    public class TransferViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; private set; }
        public string FileName { get; private set; }
        public double Progress { get; private set; }
        public string FileSize { get; private set; }
        public string TransferRate { get; private set; }
        public string ElapsedTime { get; private set; }
        public string TimeRemaining { get; private set; }
        public string Status { get; private set; }

        public FileTransfer FileTransfer { get; private set; }

        public TransferViewModel(HostState hostState)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;
        }

        public void Update(FileTransfer fileTransfer)
        {
            FileTransfer = fileTransfer;

            Project = fileTransfer.ProjectName;
            FileName = fileTransfer.Name;
            Progress = FileTransfer.NumberOfBytes > 0 ? fileTransfer.BytesTransferred / FileTransfer.NumberOfBytes : 0;
            FileSize = Utils.ConvertBytesToFileSize(fileTransfer.NumberOfBytes);
            TransferRate = FileTransfer.TransferActive ? $"{Utils.ConvertBytesToFileSize(fileTransfer.TransferSpeed)} /s" : null;
            ElapsedTime = Utils.ConvertDuration(fileTransfer.TimeSoFar);
            TimeRemaining = FileTransfer.TransferActive ? Utils.GetTimeRemaining(fileTransfer) : null;
            Status = Statuses.GetTransferStatus(fileTransfer);
        }
    }
}
