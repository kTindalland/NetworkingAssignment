using Interfaces.Shared;
using Shared.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class DisconnectMessage : IMessage
    {
        public byte Id { get; private set; }

        public DisconnectMessage()
        {
            Id = (int)MessageIds.Disconnect;
        }

        public byte[] Pack()
        {
            return new byte[] { Id };
        }

        public void Unpack(byte[] bytes)
        {
            Id = bytes[0];
        }
    }
}
