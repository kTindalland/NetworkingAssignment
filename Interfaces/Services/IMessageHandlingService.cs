using Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IMessageHandlingService
    {
        void HandleMessage(byte[] message, Socket socket);
    }
}
