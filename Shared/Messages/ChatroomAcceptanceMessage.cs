using Interfaces.Shared;
using Shared.Enumerations;
using Shared.Utilities;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class ChatroomAcceptanceMessage : IMessage
    {
        public byte Id { get; private set; }
        public byte Accepted { get; set; }
        public string ReasonForDecline { get; set; }

        public string MotD { get; set; }
        public string WelcomeMessage { get; set; }

        public ChatroomAcceptanceMessage()
        {
            Id = (int)MessageIds.ChatroomAcceptance;
            Accepted = 0;
            MotD = "";
            WelcomeMessage = "";
            ReasonForDecline = "";
        }

        public byte[] Pack()
        {
            var bytes = new List<byte>() { Id, Accepted };

            bytes.AddRange(StringPacker.PackString(ReasonForDecline));
            bytes.AddRange(StringPacker.PackString(MotD));
            bytes.AddRange(StringPacker.PackString(WelcomeMessage));

            return bytes.ToArray();
        }

        public void Unpack(byte[] bytes)
        {
            Id = bytes[0];
            Accepted = bytes[1];

            ReasonForDecline = StringPacker.UnpackString(bytes.Skip(2).ToArray(), out bytes);
            MotD = StringPacker.UnpackString(bytes, out bytes);
            WelcomeMessage = StringPacker.UnpackString(bytes, out bytes);
        }
    }
}
