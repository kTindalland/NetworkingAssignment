using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Services;
using Interfaces.Shared;

namespace Shared.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private Queue<IMessage> _messageQueue;

        public bool MessageAvailible => _messageQueue.Count > 0;

        public MessageQueueService()
        {
            _messageQueue = new Queue<IMessage>();
        }

        public IMessage PopMessage()
        {
            return _messageQueue.Dequeue();
        }

        public void QueueMessage(IMessage message)
        {
            _messageQueue.Enqueue(message);
        }
    }
}
