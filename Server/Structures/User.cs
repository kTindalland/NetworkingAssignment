using Interfaces.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Structures
{
    public class User : IUser
    {
        public Socket Socket { get; set; }
        public Stream Stream { get; set; }
        public string Username { get; set; }
        public int MissedHeartbeats { get; set; }


    }
}
