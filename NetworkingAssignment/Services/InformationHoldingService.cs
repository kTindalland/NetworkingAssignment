using Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingAssignment.Services
{
    public class InformationHoldingService : IInformationHoldingService
    {
        public List<string> ActiveUsers { get; set; }

        public InformationHoldingService()
        {
            ActiveUsers = new List<string>();
        }
    }
}
