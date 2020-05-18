using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Interfaces.Services;
using Shared.Messages;

namespace NetworkingAssignment.Services
{
    public class NetworkClientService : INetworkClientService
    {
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
            var data = msg.Pack();

            await stream.WriteAsync(data, 0, data.Length);
            stream.Flush();
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

                    //Task.Run(() => _messageHandlingService.HandleMessage(actualMessage, socket));

                    //Console.WriteLine($"> Message received: username {resultingMessage.GetType()}");

                    await flushTask;
                }

                // Send out heartbeat
                var heartbeat = new HeartbeatMessage();
                var beatData = heartbeat.Pack();
                await stream.WriteAsync(beatData, 0, beatData.Length);
                await stream.FlushAsync();
                Thread.Sleep(1000);
                
            }

            client.Close();

            

        }
    }
}
