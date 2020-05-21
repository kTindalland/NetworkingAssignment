using Interfaces.Shared;
using Shared.Enumerations;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class SendChatMessage : IMessage
    {
        public byte Id { get; private set; }
        public string Username { get; set; }
        public string Message { get; set; }

        public SendChatMessage()
        {
            Id = (int)MessageIds.SendChat;
            Username = "";
            Message = "";
        }

        public byte[] Pack()
        {
            var result = new List<byte>();
            result.Add(Id);
            result.AddRange(StringPacker.PackString(Username));
            result.AddRange(StringPacker.PackString(Message));

            return result.ToArray();
        }

        public void Unpack(byte[] bytes)
        {
            Id = bytes[0];
            Username = StringPacker.UnpackString(bytes.Skip(1).ToArray(), out bytes);
            Message = StringPacker.UnpackString(bytes, out bytes);
        }
    }
}
