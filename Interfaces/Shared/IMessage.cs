using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Shared
{
    public interface IMessage
    {
        byte[] Pack();
        void Unpack(byte[] bytes);
        byte Id { get; }
    }
}
