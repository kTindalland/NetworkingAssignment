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
        private readonly IUserTrackerService _userTracker;

        public MessageHandlingService(IUserTrackerService userTracker)
        {
            _userTracker = userTracker;
        }

        public void HandleMessage(byte[] message, Socket socket, NetworkStream stream)
        {
            IMessage decodedMessage;
            switch((MessageIds)message[0])
            {
                case MessageIds.Heartbeat:
                    decodedMessage = new HeartbeatMessage();
                    decodedMessage.Unpack(message);
                    TakeAction((HeartbeatMessage)decodedMessage, socket);
                    break;

                case MessageIds.JoinChatroom:
                    decodedMessage = new JoinChatroomMessage();
                    decodedMessage.Unpack(message);
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


            // Send accept/decline message
            var msg = new ChatroomAcceptanceMessage() { Accepted = 1, MotD = "MotD", WelcomeMessage = "Welcome to the server!" };

            lock (_userTracker.TrackerLock)
            {
                if (_userTracker.Users.Any(r => r.Value.Username == message.Username)) {
                    msg.Accepted = 0;
                    msg.ReasonForDecline = "Already a user with that username.";
                }
                else
                {
                    _userTracker.Users.Add(socket, newUser);
                }

                msg.ActiveUsers = _userTracker.Users.Select(r => r.Value.Username).ToList();
            }

            

            var buffer = msg.Pack();
            stream.Write(buffer, 0, buffer.Length);
            
        }
    }
}
