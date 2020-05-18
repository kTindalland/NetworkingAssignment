using Interfaces.Services;
using Interfaces.Shared;
using Shared.Enumerations;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public class MessageDecoderService : IMessageDecoderService
    {
        public IMessage DecodeMessage(byte[] data)
        {
            var id = (MessageIds)data[0];
            IMessage result;

            switch(id)
            {
                case MessageIds.Heartbeat:
                    result = new HeartbeatMessage();
                    break;

                case MessageIds.JoinChatroom:
                    result = new JoinChatroomMessage();
                    break;

                default:
                    result = new HeartbeatMessage();
                    break;
            }

            result.Unpack(data);

            return result;
        }
    }
}
