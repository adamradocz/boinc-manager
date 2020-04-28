using System;
using System.Collections.Generic;
using BoincManager.Interfaces;

namespace BoincManager.Models
{
    public class ObservableMessage : BindableBase, IMessage, IFilterable, IEquatable<ObservableMessage>
    {
        public int HostId { get; }
        public string HostName { get; }
        public string Project { get; }
        public string Date { get; }
        public string MessageBody { get; }
        public string Priority { get; }

        public BoincRpc.Message RpcMessage { get; private set; }

        public ObservableMessage(HostState hostState, BoincRpc.Message rpcMessage)
        {
            HostId = hostState.Id;
            HostName = hostState.Name;

            RpcMessage = rpcMessage;

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

        #region Equality comparisons
        /* From:
         * - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
         * - https://intellitect.com/overidingobjectusingtuple/
         * - https://montemagno.com/optimizing-c-struct-equality-with-iequatable/
        */

        public bool Equals(ObservableMessage other)
        {
            // If parameter is null, return false.
            if (other is null)
                return false;

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
                return true;

            return HostId == other.HostId
                && Project.Equals(other.Project, StringComparison.Ordinal)
                && RpcMessage.SequenceNumber == other.RpcMessage.SequenceNumber;
        }

        public override bool Equals(object obj)
        {
            ObservableMessage message = obj as ObservableMessage;
            return message == null ? false : Equals(message);
        }

        public override int GetHashCode()
        {            
            return HashCode.Combine(HostId, Project, RpcMessage.SequenceNumber);
        }

        public static bool operator ==(ObservableMessage lhs, ObservableMessage rhs)
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

        public static bool operator !=(ObservableMessage lhs, ObservableMessage rhs)
        {
            return !(lhs == rhs);
        }
        #endregion
    }
}
