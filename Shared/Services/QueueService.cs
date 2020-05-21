using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Services;
using Interfaces.Shared;

namespace Shared.Services
{
    public class QueueService<T> : IQueueService<T>
    {
        private Queue<T> _queue;

        public bool ItemAvailable => _queue.Count > 0;
        public object QueueLock { get; }

        public QueueService()
        {
            _queue = new Queue<T>();
            QueueLock = new object();
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public void Enqueue(T message)
        {
            _queue.Enqueue(message);
        }
    }
}
