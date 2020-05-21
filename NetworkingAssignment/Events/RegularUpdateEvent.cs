using Prism.Events;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingAssignment.Events
{
    public class RegularUpdateEvent : PubSubEvent<RegularUpdateMessage>
    {
    }
}
