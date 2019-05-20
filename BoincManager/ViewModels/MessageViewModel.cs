using BoincRpc;

namespace BoincManager.ViewModels
{
    public class MessageViewModel
    {
        public int HostId { get; }
        public string HostName { get; private set; }
        public string Project { get; private set; }
        public string Date { get; private set; }
        public string Message { get; private set; }
        public string Priority { get; private set; }

        public Message RpcMessage { get; private set; }

        public MessageViewModel(int hostId)
        {
            HostId = hostId;
        }

        public void Update(Message message, string computerName)
        {
            RpcMessage = message;

            Project = message.Project;
            Date = message.Timestamp.ToLocalTime().ToString("g");
            Message = message.Body;
            Priority = message.Priority.ToString();

            HostName = computerName;
        }
    }
}
