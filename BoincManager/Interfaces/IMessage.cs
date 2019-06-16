namespace BoincManager.Interfaces
{
    public interface IMessage
    {
        int HostId { get; }
        string HostName { get; }
        string Project { get; }
        string Date { get; }
        string MessageBody { get; }
        string Priority { get; }
    }
}
