using Interfaces.Services;
using Interfaces.Shared;
using NetworkingAssignment.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkingAssignment.ViewModels
{
    public class ChatRoomViewModel : BindableBase
    {
		private readonly IEventAggregator _eventAggregator;
		private readonly IInformationHoldingService _informationHolding;
		private readonly IQueueService<IMessage> _queueService;

		public ICommand SendChatCommand { get; set; }

		private List<string> _messages;
		

		public List<string> Messages
		{
			get { return _messages; }
			set 
			{
				_messages = value;
				RaisePropertyChanged();
			}
		}

		private List<string> _activeUsers;

		public List<string> ActiveUsers
		{
			get { return _activeUsers; }
			set
			{ 
				_activeUsers = value;
				RaisePropertyChanged();
			}
		}


		private string _text;

		public string Text
		{
			get { return _text; }
			set 
			{ 
				_text = value;
				RaisePropertyChanged();
			}
		}


		public ChatRoomViewModel(
			IEventAggregator eventAggregator,
			IInformationHoldingService informationHolding,
			IQueueService<IMessage> queueService)
		{
			_eventAggregator = eventAggregator;
			_informationHolding = informationHolding;
			_queueService = queueService;
			_eventAggregator.GetEvent<RegularUpdateEvent>().Subscribe(OnRegularUpdate);

			Messages = new List<string>();
			ActiveUsers = _informationHolding.ActiveUsers;

			SendChatCommand = new DelegateCommand(SendChat);
		}

		private void SendChat()
		{
			var newMessage = new SendChatMessage()
			{
				Message = Text,
				Username = _informationHolding.Username
			};

			_queueService.Enqueue(newMessage);
		}

		public void OnRegularUpdate(RegularUpdateMessage payload)
		{
			if (payload.NewChats.Count > 0)
			{
				var tempList = new List<string>();
				tempList.AddRange(Messages);

				foreach (var chat in payload.NewChats)
				{
					tempList.Add($"{chat.Username} : {chat.Message}");
				}
				Messages = tempList;
			}
			
			if (payload.ActiveUsers.Count != ActiveUsers.Count)
			{
				ActiveUsers = payload.ActiveUsers;
			}
		}
	}
}
