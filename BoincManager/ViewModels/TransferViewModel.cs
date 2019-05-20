using BoincRpc;

namespace BoincManager.ViewModels
{
    public class TransferViewModel
    {
        public int HostId { get; }
        public string HostName { get; private set; }
        public string Project { get; set; }
        public string FileName { get; set; }
        public double Progress { get; set; }
        public string FileSize { get; set; }
        public string TransferRate { get; set; }
        public string ElapsedTime { get; set; }
        public string TimeRemaining { get; set; }
        public string Status { get; set; }

        public FileTransfer FileTransfer { get; private set; }

        public TransferViewModel(int hostId, string hostName)
        {
            HostId = hostId;
            HostName = hostName;
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
