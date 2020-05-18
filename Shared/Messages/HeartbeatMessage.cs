using Interfaces.Shared;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class HeartbeatMessage : IMessage
    {
        public byte Id { get; private set; }

        public HeartbeatMessage()
        {
            Id = 1;
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
