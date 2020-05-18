using Interfaces.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Interfaces.Services
{
    public interface IUserTrackerService
    {
        object TrackerLock { get; set; }
        Dictionary<Socket, IUser> Users { get; set; }
    }
}
