using Interfaces.Shared;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Enumerations;

namespace Shared.Messages
{
    public class HeartbeatMessage : IMessage
    {
        public byte Id { get; private set; }

        public HeartbeatMessage()
        {
            Id = (int)MessageIds.Heartbeat;
        }

        public byte[] Pack()
        {
            var packedBytes = new byte[] { Id };

            return packedBytes;
        }

        public void Unpack(byte[] bytes)
        {
            return;
        }
    }
}
