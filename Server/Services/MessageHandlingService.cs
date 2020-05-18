using Interfaces.Services;
using Interfaces.Shared;
using Server.Structures;
using Shared.Enumerations;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    // SERVER MESSAGE HANDLING
    public class MessageHandlingService : IMessageHandlingService
    {
        private readonly IMessageDecoderService _messageDecoder;
        private readonly IUserTrackerService _userTracker;

        public MessageHandlingService(IMessageDecoderService messageDecoder, IUserTrackerService userTracker)
        {
            _messageDecoder = messageDecoder;
            _userTracker = userTracker;
        }

        public void HandleMessage(byte[] message, Socket socket)
        {
            var decodedMessage = _messageDecoder.DecodeMessage(message);


            switch((MessageIds)decodedMessage.Id)
            {
                case MessageIds.Heartbeat:
                    TakeAction((HeartbeatMessage)decodedMessage, socket);
                    break;

                case MessageIds.JoinChatroom:
                    TakeAction((JoinChatroomMessage)decodedMessage, socket);
                    break;

                default:
                    break;
            }
        }

        private void TakeAction(HeartbeatMessage message, Socket socket)
        {
            

            lock (_userTracker.TrackerLock)
            {
                _userTracker.Users[socket].MissedHeartbeats = 0;

                var username = _userTracker.Users[socket].Username;
                Console.WriteLine($"Heartbeat from {username}");
            }
            
        }

        private void TakeAction(JoinChatroomMessage message, Socket socket)
        {
            var newUser = new User()
            {
                Socket = socket,
                Username = message.Username,
                MissedHeartbeats = 0
            };

            lock(_userTracker.TrackerLock)
            {
                _userTracker.Users.Add(socket, newUser);
            }
            
        }
    }
}
