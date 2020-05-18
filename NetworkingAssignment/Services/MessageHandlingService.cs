using Interfaces.Services;
using Shared.Enumerations;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NetworkingAssignment.Services
{
    public class MessageHandlingService : IMessageHandlingService
    {
        private readonly IMessageDecoderService _decoderService;

        public MessageHandlingService(IMessageDecoderService decoderService)
        {
            _decoderService = decoderService;
        }

        public void HandleMessage(byte[] message, Socket socket)
        {
            var decodedMessage = _decoderService.DecodeMessage(message);


            switch ((MessageIds)decodedMessage.Id)
            {
                case MessageIds.ChatroomAcceptance:
                    TakeAction((ChatroomAcceptanceMessage)decodedMessage);
                    break;

                default:
                    break;
            }
        }

        private void TakeAction(ChatroomAcceptanceMessage message)
        {

        }
    }
}
