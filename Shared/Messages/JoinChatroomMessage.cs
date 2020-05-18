using Interfaces.Shared;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class JoinChatroomMessage : IMessage
    {
        public byte Id { get; private set; }
        public string Username { get; set; }

        public JoinChatroomMessage()
        {
            Id = 2;
        }

        public byte[] Pack()
        {
            var package = new List<byte>();
            package.Add(Id);
            package.AddRange(StringPacker.PackString(Username));

            return package.ToArray();
        }

        public void Unpack(byte[] bytes)
        {
            Id = bytes[0];
            var buffer = bytes;
            Username = StringPacker.UnpackString(bytes.Skip(1).ToArray(), out buffer);
        }
    }
}
