using BoincManager.Interfaces;
using System;
using System.Collections.Generic;

namespace BoincManager.Models
{
    public class ObservableTransfer : BindableBase, ITransfer, IFilterable, IEquatable<ObservableTransfer>
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


        #region Equality comparisons
        /* From:
         * - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
         * - https://intellitect.com/overidingobjectusingtuple/
         * - https://montemagno.com/optimizing-c-struct-equality-with-iequatable/
        */

        public bool Equals(ObservableTransfer other)
        {
            // If parameter is null, return false.
            if (other is null)
                return false;

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
                return true;

            return HostId == other.HostId
                && Project.Equals(other.Project, StringComparison.Ordinal)
                && FileName.Equals(other.FileName, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            ObservableTransfer transfer = obj as ObservableTransfer;
            return transfer == null ? false : Equals(transfer);
        }

        public override int GetHashCode()
        {
            //return HashCode.Combine(HostId, RpcResult.Name); Available in .NET Strandard 2.1, but the current Xamarin version doesn't support it, only .NET Standard 2.0
            return (HostId, Project, FileName).GetHashCode();
        }

        public static bool operator ==(ObservableTransfer lhs, ObservableTransfer rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ObservableTransfer lhs, ObservableTransfer rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
