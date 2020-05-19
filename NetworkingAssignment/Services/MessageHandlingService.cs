using Interfaces.Services;
using Interfaces.Shared;
using NetworkingAssignment.Events;
using Prism.Events;
using Prism.Regions;
using Shared.Enumerations;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NetworkingAssignment.Services
{
    public class MessageHandlingService : IMessageHandlingService
    {
        private readonly IMessageQueueService _queueService;
        private readonly IEventAggregator _eventAggregator;
        private readonly object _heartbeatLock;
        private bool _alive;

        public MessageHandlingService(
            IMessageQueueService queueService,
            IEventAggregator eventAggregator)
        {
            _queueService = queueService;
            _eventAggregator = eventAggregator;
            _alive = false;
            _heartbeatLock = new object();

            _eventAggregator.GetEvent<KillHeartbeatEvent>().Subscribe(OnKillHeartbeater);
        }

        public void HandleMessage(byte[] message, Socket socket)
        {
            IMessage decodedMessage;
            switch ((MessageIds)message[0])
            {
                case MessageIds.ChatroomAcceptance:
                    decodedMessage = new ChatroomAcceptanceMessage();
                    decodedMessage.Unpack(message);
                    TakeAction((ChatroomAcceptanceMessage)decodedMessage);
                    break;

                default:
                    break;
            }
        }

        private void TakeAction(ChatroomAcceptanceMessage message)
        {
            // Set heartbeat going

            _eventAggregator.GetEvent<ChatroomAcceptanceEvent>().Publish(message);

            if (message.Accepted == 1)
            {
                lock (_heartbeatLock)
                {
                    _alive = true;
                }
                Task.Run(() => Heartbeater());
            }
        }

        private async Task Heartbeater()
        {
            while (true)
            {
                lock (_heartbeatLock)
                {
                    if (!_alive)
                    {
                        return;
                    }
                }

                // Thump thump
                var beat = new HeartbeatMessage();

                lock (_queueService.QueueLock)
                {
                    _queueService.QueueMessage(beat);
                }


                Thread.Sleep(1000);
            }
        }

        private void OnKillHeartbeater()
        {
            lock (_heartbeatLock)
            {
                _alive = false;
            }
        }
    }
}
