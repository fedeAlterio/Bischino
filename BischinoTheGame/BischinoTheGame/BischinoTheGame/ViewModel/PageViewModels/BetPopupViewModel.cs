using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Queries;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class BetPopupViewModel : PageViewModel
    {
        private readonly MatchSnapshot _snapshot;
        private readonly string _roomName;
        private int? _missingNumber;


        // Initialization
        public BetPopupViewModel(string roomName, MatchSnapshot snapshot)
        {
            _snapshot = snapshot;
            _roomName = roomName;
            BetViewModel = _snapshot.Player.BetViewModel;
            BetCommand = NewCommand<int>(Bet);
            InfoCommand = NewCommand(ToInfoPopup);
            CheckInfoVisible();
        }
        


        // Properties
        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get => _isInfoVisible;
            set => SetProperty(ref _isInfoVisible, value);
        }


        private BetViewModel _betViewModel;
        public BetViewModel BetViewModel
        {
            get => _betViewModel;
            set => SetProperty(ref _betViewModel, value);
        }


        // Commands
        public IAsyncCommand<int> BetCommand { get; }
        public IAsyncCommand InfoCommand { get; }



        private async Task ToInfoPopup()
        {
            if(_missingNumber is null)
                throw new Exception("Missing number cannot be null");

            await AppController.Navigation.GameNavigation.ToBetInfoPopup(_missingNumber.Value);
        }

        private void CheckInfoVisible()
        {
            var cardsCount = _snapshot.Player.StartCardsCount;
            var range = Enumerable.Range(0, cardsCount + 1).Except(BetViewModel.PossibleBets);
            
            if (range.Any())
                _missingNumber = range.FirstOrDefault();
            IsInfoVisible = _missingNumber != null;
        }


        // Commands handlers
        private async Task Bet(int bet)
        {
            var query = new RoomQuery<int> { PlayerName = _snapshot.Player.Name, RoomName = _roomName, Data = bet };
            await AppController.GameHandler.MakeABet(query);
            await AppController.Navigation.GameNavigation.NotifyBetCompleted();
        }
    }
}
