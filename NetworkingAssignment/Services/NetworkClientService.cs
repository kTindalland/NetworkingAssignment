using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Interfaces.Services;
using Interfaces.Shared;
using NetworkingAssignment.Events;
using Prism.Events;
using Shared.Messages;

namespace NetworkingAssignment.Services
{
    public class NetworkClientService : INetworkClientService
    {
        private readonly IMessageQueueService _queueService;
        private readonly IMessageHandlingService _messageHandlingService;
        private readonly IEventAggregator _eventAggregator;

        public NetworkClientService(
            IMessageQueueService queueService,
            IMessageHandlingService messageHandlingService,
            IEventAggregator eventAggregator)
        {
            _queueService = queueService;
            _messageHandlingService = messageHandlingService;
            _eventAggregator = eventAggregator;
        }

        public bool Connect(string ipAddress, string port, string username)
        {
            Task.Run(() => ConnectTask(ipAddress, port, username));

            return true;
        }

        private async Task ConnectTask(string ipAddress, string port, string username)
        {
            var ip = IPAddress.Parse(ipAddress);
            var intPort = int.Parse(port);

            var ipEndpoint = new IPEndPoint(ip, intPort);

            var client = new TcpClient();
            client.Connect(ipEndpoint);

            var stream = client.GetStream();

            // for testing purposes send over join chat room message
            var msg = new JoinChatroomMessage() { Username = username };
            lock (_queueService.QueueLock)
            {
                _queueService.QueueMessage(msg);
            }

            var socket = client.Client;
            while (!(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0))
            {
                if (stream.DataAvailable)
                {
                    int twoKiloBytes = 2048;
                    byte[] buffer = new byte[twoKiloBytes];

                    int messageLength = await stream.ReadAsync(buffer, 0, twoKiloBytes);
                    var flushTask = stream.FlushAsync();

                    byte[] actualMessage = buffer.Take(messageLength).ToArray();

                    await Task.Run(() => _messageHandlingService.HandleMessage(actualMessage, socket));

                    //Console.WriteLine($"> Message received: username {resultingMessage.GetType()}");

                    await flushTask;
                }

                if (_queueService.MessageAvailible)
                {
                    // Message to send
                    IMessage message;
                    lock (_queueService.QueueLock)
                    {
                        message = _queueService.PopMessage();
                    }

                    var bytes = message.Pack();
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await stream.FlushAsync();
                    
                }

            }

            client.Close();

            Debug.WriteLine("Got here!");
            _eventAggregator.GetEvent<KillHeartbeatEvent>().Publish();


        }
    }
}
