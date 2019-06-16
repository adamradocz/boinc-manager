namespace BoincManager.Interfaces
{
    public interface IHost
    {
        int Id { get; }        
        string Name { get; }
        string IpAddress { get; }
        int Port { get; }
        string Password { get; }
        bool AutoConnect { get; }
        string BoincVersion { get; }
        string OperatingSystem { get; }
        bool Connected { get; }
        string Status { get; }
    }
}
