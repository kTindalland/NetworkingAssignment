using Interfaces.Services;
using Interfaces.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class UserTrackerService : IUserTrackerService
    {

        private Dictionary<Socket, IUser> _users;
        public Dictionary<Socket, IUser> Users
        {
            get
            {
                return _users;
            }

            set
            {
                 _users = value;
            }
        }

        public object TrackerLock { get; set; }

        public UserTrackerService()
        {
            TrackerLock = new object();
            Users = new Dictionary<Socket, IUser>();
        }
    }
}
