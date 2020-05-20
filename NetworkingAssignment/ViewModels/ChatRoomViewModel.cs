using Interfaces.Services;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkingAssignment.ViewModels
{
    public class ChatRoomViewModel : BindableBase
    {

		private List<string> _myList;
		private readonly IEventAggregator _eventAggregator;
		private readonly IInformationHoldingService _informationHolding;

		public List<string> MyList
		{
			get { return _myList; }
			set 
			{
				_myList = value;
				RaisePropertyChanged();
			}
		}


		public ChatRoomViewModel(
			IEventAggregator eventAggregator,
			IInformationHoldingService informationHolding)
		{
			_eventAggregator = eventAggregator;
			_informationHolding = informationHolding;

			MyList = _informationHolding.ActiveUsers;
		}
	}
}
