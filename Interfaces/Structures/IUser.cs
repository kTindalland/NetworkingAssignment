using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Structures
{
    public interface IUser
    {
        Socket Socket { get; set; }
        Stream Stream { get; set; }
        string Username { get; set; }
        int MissedHeartbeats { get; set; }
    }
}
