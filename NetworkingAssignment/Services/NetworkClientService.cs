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
            var myReader = new StreamReader(stream);
            var myWriter = new StreamWriter(stream);
            myWriter.AutoFlush = true;

            Debug.WriteLine(myReader.ReadLine());



            for (int i = 0; i < 5; i++)
            {
                myWriter.WriteLine($"{username} : {i.ToString()}");
                Thread.Sleep(100);
            }


            client.Close();

            

        }
    }
}
