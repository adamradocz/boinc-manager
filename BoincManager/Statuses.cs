using BoincManager.Models;
using BoincRpc;
using System.Text;

namespace BoincManager
{
    public static class Statuses
    {
        /// <summary>
        /// Get the status of a project.
        /// </summary>
        /// <param name="project">Boinc RPC Project.</param>
        /// <returns></returns>
        public static string GetProjectStatus(BoincRpc.Project project)
        {
            StringBuilder status = new StringBuilder();

            if (project.Ended)
                status.Append("Ended, ");
            if (project.DetachWhenDone)
                status.Append("Detach when done, ");
            if (project.Suspended)
                status.Append("Suspended, ");
            if (project.DontRequestMoreWork)
                status.Append("Don't request more work, ");
            if (project.SchedulerRpcInProgress)
                status.Append("Scheduler request in progress, ");
            else if (project.SchedulerRpcPending != 0)
                status.Append("Scheduler request pending, ");
            if (project.MasterUrlFetchPending)
                status.Append("Master URL fetch pending, ");

            return status.ToString().TrimEnd(',', ' ');
        }

        /// <summary>
        /// Get the status of a task (Working unit)
        /// </summary>
        /// <param name="result">Boinc RPC Result. AKA Working unit/Task</param>
        /// <param name="rpcProject"></param>
        /// <param name="boincState"></param>
        /// <returns></returns>
        public static string GetTaskStatus(Result result, BoincRpc.Project rpcProject, BoincState boincState)
        {
            // clientgui/MainDocument.cpp, result_description()

            if (rpcProject == null)
                return "Couldn't update TaskViewModels maybe CoreClientState update is required.";

            switch (result.State)
            {
                case ResultState.New:
                    return string.Empty;
                case ResultState.FilesDownloading:
                    if (result.ReadyToReport)
                        return "Download failed";
                    if (boincState.CoreClientStatus.NetworkSuspendReason == SuspendReason.None)
                        return "Downloading";
                    else
                        return string.Empty;
                case ResultState.FilesDownloaded:
                    if (result.ProjectSuspended)
                        return "Project suspended";
                    if (result.Suspended)
                        return "Suspended";
                    if (boincState.CoreClientStatus.TaskSuspendReason != SuspendReason.None &&
                        boincState.CoreClientStatus.TaskSuspendReason != SuspendReason.CpuThrottle)
                        return string.Empty;
                    if (boincState.CoreClientStatus.GpuSuspendReason != SuspendReason.None &&
                        result.Resources != null && result.Resources.Contains("GPU"))
                        return string.Empty;
                    // TODO: check result.scheduler_wait and network_wait
                    if (result.ActiveTask)
                    {
                        if (result.TooLarge || result.NeedsSharedMemory)
                            return string.Empty;
                        switch (result.SchedulerState)
                        {
                            case SchedulerState.Scheduled:
                                return "Running";
                            case SchedulerState.Preempted:
                                return string.Empty;
                            case SchedulerState.Uninitialized:
                            default:
                                return string.Empty;
                        }
                    }
                    else
                        return string.Empty;
                case ResultState.ComputeError:
                    return "Computation error";
                case ResultState.FilesUploading:
                    if (result.ReadyToReport)
                        return "Upload failed";
                    if (boincState.CoreClientStatus.NetworkSuspendReason == SuspendReason.None)
                        return "Uploading";
                    else
                        return string.Empty;
                case ResultState.Aborted:
                    switch ((ExitCode)result.ExitStatus)
                    {
                        case ExitCode.AbortedViaGui:
                            return "Aborted";
                        case ExitCode.AbortedByProject:
                            return "Aborted by project";
                        case ExitCode.UnstartedLate:
                            return "Aborted: not started by deadline";
                        case ExitCode.DiskLimitExceeded:
                            return "Aborted: disk limit exceeded";
                        case ExitCode.TimeLimitExceeded:
                            return "Aborted: time limit exceeded";
                        case ExitCode.MemoryLimitExceeded:
                            return "Aborted: memory limit exceeded";
                        default:
                            return "Aborted";
                    }
                default:
                    if (result.GotServerAck)
                        return string.Empty;
                    if (result.ReadyToReport)
                        return "Finished";
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get the status of a file transfer.
        /// </summary>
        /// <param name="fileTransfer">Boinc RPC FileTransfer</param>
        /// <returns></returns>
        public static string GetTransferStatus(FileTransfer fileTransfer)
        {
            StringBuilder status = new StringBuilder();

            if (fileTransfer.TransferActive)
                status.Append("Active, ");
            if (fileTransfer.NumberOfRetries > 0)
            {
                string suffix;

                if (fileTransfer.NumberOfRetries == 1)
                    suffix = "st";
                else if (fileTransfer.NumberOfRetries == 2)
                    suffix = "nd";
                else if (fileTransfer.NumberOfRetries == 3)
                    suffix = "rd";
                else
                    suffix = "th";

                status.AppendFormat("{0}{1} retry, ", fileTransfer.NumberOfRetries, suffix);
            }

            return status.ToString().TrimEnd(',', ' ');
        }
    }
}
