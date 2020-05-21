using Interfaces.Shared;
using Shared.Enumerations;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public struct Chat
    {
        public string Username { get; set; }
        public string Message { get; set; }
    }

    public class RegularUpdateMessage : IMessage
    {
        public byte Id { get; private set; }
        public List<string> ActiveUsers { get; set; }
        public List<Chat> NewChats { get; set; }

        public RegularUpdateMessage()
        {
            Id = (int)MessageIds.RegularUpdate;
            ActiveUsers = new List<string>();
            NewChats = new List<Chat>();
        }
        
        public byte[] Pack()
        {
            var result = new List<byte>()
            {
                Id
            };

            result.AddRange(StringPacker.PackList(ActiveUsers));

            var chatList = new List<string>();

            foreach (var chat in NewChats)
            {
                chatList.Add(chat.Username);
                chatList.Add(chat.Message);
            }

            result.AddRange(StringPacker.PackList(chatList));

            return result.ToArray();
        }

        public void Unpack(byte[] bytes)
        {
            Id = bytes[0];

            ActiveUsers = StringPacker.UnpackList(bytes.Skip(1).ToArray(), out bytes);

            var unparsedList = StringPacker.UnpackList(bytes, out bytes);
            NewChats = new List<Chat>();

            for (int i = 0; i < unparsedList.Count; i += 2)
            {
                var newChat = new Chat()
                {
                    Username = unparsedList[i],
                    Message = unparsedList[i + 1]
                };

                NewChats.Add(newChat);
            }
        }
    }
}
