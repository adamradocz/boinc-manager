using BoincManager;
using BoincManager.Models;
using BoincRpc;

namespace BoincManagerWeb.ViewModels
{
    public class TransferViewModel
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string FileName { get; }
        public double Progress { get; }
        public string FileSize { get; }
        public string TransferRate { get; }
        public string ElapsedTime { get; }
        public string TimeRemaining { get; }
        public string Status { get; }

        public TransferViewModel(HostState hostState, FileTransfer fileTransfer)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            Project = fileTransfer.ProjectName;
            FileName = fileTransfer.Name;
            Progress = fileTransfer.NumberOfBytes > 0 ? fileTransfer.BytesTransferred / fileTransfer.NumberOfBytes : 0;
            FileSize = Utils.ConvertBytesToFileSize(fileTransfer.NumberOfBytes);
            TransferRate = fileTransfer.TransferActive ? $"{Utils.ConvertBytesToFileSize(fileTransfer.TransferSpeed)} /s" : string.Empty;
            ElapsedTime = Utils.ConvertDuration(fileTransfer.TimeSoFar);
            TimeRemaining = fileTransfer.TransferActive ? Utils.GetTimeRemaining(fileTransfer) : string.Empty;
            Status = Statuses.GetTransferStatus(fileTransfer);
        }

    }
}
