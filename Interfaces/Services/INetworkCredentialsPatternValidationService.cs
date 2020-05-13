using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface INetworkCredentialsPatternValidationService
    {
        bool ValidateIpAddressPattern(string ipAddress);
        bool ValidatePortPattern(string port);
        bool ValidateUsernamePattern(string username);
    }
}
