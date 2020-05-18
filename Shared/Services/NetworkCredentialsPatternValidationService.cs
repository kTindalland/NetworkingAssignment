using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Interfaces.Services;

namespace Shared.Services
{
    public class NetworkCredentialsPatternValidationService : INetworkCredentialsPatternValidationService
    {
        public bool ValidateIpAddressPattern(string ipAddress)
        {
            if (ipAddress == null) return false;

            var pattern = new Regex("^([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])\\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])$");
            
            var result = pattern.IsMatch(ipAddress);

            return result;
        }

        public bool ValidatePortPattern(string port)
        {
            if (port == null) return false;

            var pattern = new Regex("^\\d+$");
            var result = pattern.IsMatch(port);

            if (!result) return false;

            var intPort = int.Parse(port);

            if (intPort > 65535)
            {
                result = false;
            }

            return result;
        }

        public bool ValidateUsernamePattern(string username)
        {
            if (username == null) return false;

            if (username.Length < 1) return false;

            return true;
        }
    }
}
