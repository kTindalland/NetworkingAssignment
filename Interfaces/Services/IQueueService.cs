using Interfaces.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IQueueService<T>
    {
        bool ItemAvailable { get; }
        object QueueLock { get; }
        void Enqueue(T message);
        T Dequeue();

    }
}
