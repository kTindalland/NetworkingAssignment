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
    public class MessageHandlingService : IServerMessageHandlingService
    {
        private readonly IMessageDecoderService _messageDecoder;
        private readonly IUserTrackerService _userTracker;

        public MessageHandlingService(IMessageDecoderService messageDecoder, IUserTrackerService userTracker)
        {
            _messageDecoder = messageDecoder;
            _userTracker = userTracker;
        }

        public void HandleMessage(byte[] message, Socket socket, NetworkStream stream)
        {
            var decodedMessage = _messageDecoder.DecodeMessage(message);


            switch((MessageIds)decodedMessage.Id)
            {
                case MessageIds.Heartbeat:
                    TakeAction((HeartbeatMessage)decodedMessage, socket);
                    break;

                case MessageIds.JoinChatroom:
                    TakeAction((JoinChatroomMessage)decodedMessage, socket, stream);
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
            }
            
        }

        private void TakeAction(JoinChatroomMessage message, Socket socket, NetworkStream stream)
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

            // Send accept message
            var msg = new ChatroomAcceptanceMessage() { Accepted = 1, MotD = "MotD", WelcomeMessage = "Welcome to the server!" };
            var buffer = msg.Pack();
            stream.Write(buffer, 0, buffer.Length);
            
        }
    }
}
