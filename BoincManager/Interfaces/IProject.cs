namespace BoincManager.Interfaces
{
    public interface IProject
    {
        int HostId { get; }
        string HostName { get; }
        string Name { get; }
        string Username { get; }
        string Team { get; }
        string Credit { get; }
        string AverageCredit { get; }
        string Status { get; }
    }
}
