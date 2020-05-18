using Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IMessageDecoderService
    {
        IMessage DecodeMessage(byte[] data);
    }
}
