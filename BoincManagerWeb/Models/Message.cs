using BoincManager.Interfaces;
using BoincManager.Models;
using System.Collections.Generic;

namespace BoincManagerWeb.Models
{
    public class Message : IMessage, IFilterable
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string Date { get; }
        public string MessageBody { get; }
        public string Priority { get; }

        public Message(HostState hostState, BoincRpc.Message rpcMessage)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            Project = rpcMessage.Project;
            Date = rpcMessage.Timestamp.ToLocalTime().ToString("g");
            MessageBody = rpcMessage.Body;
            Priority = rpcMessage.Priority.ToString();
        }

        public IEnumerable<string> GetContentsForFiltering()
        {
            yield return HostName;
            yield return Project;
            yield return Date;
            yield return MessageBody;
        }
    }
}
