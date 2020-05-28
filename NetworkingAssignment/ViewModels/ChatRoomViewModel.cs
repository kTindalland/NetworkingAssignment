using Interfaces.Services;
using Interfaces.Shared;
using NetworkingAssignment.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NetworkingAssignment.ViewModels
{
    public class ChatRoomViewModel : BindableBase
    {
		private readonly IEventAggregator _eventAggregator;
		private readonly IInformationHoldingService _informationHolding;
		private readonly IQueueService<IMessage> _queueService;
		private readonly IRegionManager _regionManager;

		public ICommand SendChatCommand { get; set; }
		public ICommand DisconnectCommand { get; set; }

		private readonly Dispatcher _dispatcher;
		private ObservableCollection<string> _messages;
		

		public ObservableCollection<string> Messages
		{
			get { return _messages; }
			set 
			{
				_messages = value;
				RaisePropertyChanged();
			}
		}

		private ObservableCollection<string> _activeUsers;

		public ObservableCollection<string> ActiveUsers
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

		private int _selectedIndex;

		public int SelectedIndex
		{
			get { return _selectedIndex; }
			set 
			{ 
				_selectedIndex = value;
				RaisePropertyChanged();
			}
		}



		public ChatRoomViewModel(
			IEventAggregator eventAggregator,
			IInformationHoldingService informationHolding,
			IQueueService<IMessage> queueService,
			IRegionManager regionManager)
		{
			_eventAggregator = eventAggregator;
			_informationHolding = informationHolding;
			_queueService = queueService;
			_regionManager = regionManager;
			_eventAggregator.GetEvent<RegularUpdateEvent>().Subscribe(OnRegularUpdate);
			_eventAggregator.GetEvent<KillHeartbeatEvent>().Subscribe(OnKillHeardbeat);

			Messages = new ObservableCollection<string>();
			ActiveUsers = new ObservableCollection<string>(_informationHolding.ActiveUsers);

			SendChatCommand = new DelegateCommand(SendChat);
			DisconnectCommand = new DelegateCommand(Disconnect);

			_dispatcher = Dispatcher.CurrentDispatcher;
		}

		private void SendChat()
		{
			var newMessage = new SendChatMessage()
			{
				Message = Text,
				Username = _informationHolding.Username
			};

			_queueService.Enqueue(newMessage);
			Text = "";
		}

		private void Disconnect()
		{
			_eventAggregator.GetEvent<KillHeartbeatEvent>().Publish();
		}

		public void OnRegularUpdate(RegularUpdateMessage payload)
		{
			if (payload.NewChats.Count > 0)
			{
				foreach (var chat in payload.NewChats)
				{
					_dispatcher.Invoke(() => { Messages.Add($"{chat.Username} : {chat.Message}"); });
					SelectedIndex = Messages.Count - 1;
				}
			}

			if (payload.ActiveUsers.Count != ActiveUsers.Count)
			{
				ActiveUsers = new ObservableCollection<string>(payload.ActiveUsers);
			}
		}

		private void OnKillHeardbeat()
		{
			_dispatcher.Invoke(() => { _regionManager.RequestNavigate("MainRegion", "Home"); });
		}
	}
}
