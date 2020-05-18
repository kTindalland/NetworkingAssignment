using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Structures
{
    public interface IUser
    {
        Socket Socket { get; set; }
        string Username { get; set; }
        int MissedHeartbeats { get; set; }
    }
}
