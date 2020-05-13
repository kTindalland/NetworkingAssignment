using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface INetworkClientService
    {
        bool Connect(string ipAddress, string port, string username);
    }
}
