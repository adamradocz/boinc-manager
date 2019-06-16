namespace BoincManager.Interfaces
{
    public interface ITransfer
    {
        int HostId { get; }
        string HostName { get; }
        string Project { get; }
        string FileName { get; }
        double Progress { get; }
        string FileSize { get; }
        string TransferRate { get; }
        string ElapsedTime { get; }
        string TimeRemaining { get; }
        string Status { get; }
    }
}
