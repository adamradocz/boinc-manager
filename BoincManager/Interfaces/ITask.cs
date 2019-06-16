namespace BoincManager.Interfaces
{
    public interface ITask
    {
        int HostId { get; }
        string HostName { get; }
        string Project { get; }
        string Application { get; }
        string Workunit { get; }
        double Progress { get; }
        string ElapsedTime { get; }
        string CpuTime { get; }
        string CpuTimeRemaining { get; }
        string LastCheckpoint { get; }
        string Deadline { get; }
        string Status { get; }
    }
}
