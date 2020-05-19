using Interfaces.Services;
using NetworkingAssignment.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace NetworkingAssignment.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        #region Private Attributes

        private readonly IRegionManager _regionManager;
        private readonly INetworkCredentialsPatternValidationService _patternValidationService;
        private readonly INetworkClientService _clientService;
        private readonly IEventAggregator _eventAggregator;
        private string _ipAddress;
        private string _port;
        private string _username;
        private string _errorMessage;
        private Dispatcher _mainDispatcher;

        #endregion

        #region Public Commands

        public ICommand ConnectCommand { get; set; }

        #endregion

        #region Public Properties

        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
                RaisePropertyChanged();
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                RaisePropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        public HomeViewModel(
            IRegionManager regionManager,
            INetworkCredentialsPatternValidationService patternValidationService,
            INetworkClientService clientService,
            IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _patternValidationService = patternValidationService;
            _clientService = clientService;
            _eventAggregator = eventAggregator;
            ConnectCommand = new DelegateCommand(Connect);
            ErrorMessage = "";

            _mainDispatcher = Dispatcher.CurrentDispatcher;

            _eventAggregator.GetEvent<ChatroomAcceptanceEvent>().Subscribe(OnChatroomAcceptance);
        }

        private void OnChatroomAcceptance(ChatroomAcceptanceMessage payload)
        {
            if (payload.Accepted == 1)
            {
                _mainDispatcher.Invoke(() => { _regionManager.RequestNavigate("MainRegion", "ChatRoom"); });
            }
            else
            {
                ErrorMessage = payload.ReasonForDecline;
            }
        }

        private void Connect()
        {
            // Validate inputs

            var ipNotValid = !_patternValidationService.ValidateIpAddressPattern(IpAddress);
            var portNotValid = !_patternValidationService.ValidatePortPattern(Port);

            if (ipNotValid || portNotValid)
            {
                // Flag up some error message
                ErrorMessage = "Network Credentials not valid.";
                return;
            }

            var usernameNotValid = !_patternValidationService.ValidateUsernamePattern(Username);
            if (usernameNotValid)
            {
                ErrorMessage = "Username not valid.";
                return;
            }

            ErrorMessage = "";

            _clientService.Connect(IpAddress, Port, Username);
        }
    }
}
