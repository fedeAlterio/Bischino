using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Queries;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class BetPopupViewModel : PageViewModel
    {
        private readonly MatchSnapshot _snapshot;
        private readonly string _roomName;
        private int? _missingNumber;

        private BetViewModel _betViewModel;
        public BetViewModel BetViewModel
        {
            get => _betViewModel;
            set => SetProperty(ref _betViewModel, value);
        }


        private Command<int> _betCommand;
        public Command<int> BetCommand
        {
            get => _betCommand;
            set => SetProperty(ref _betCommand, value);
        }


        private bool _isInfoVisible;
        public bool IsInfoVisible
        {
            get => _isInfoVisible;
            set => SetProperty(ref _isInfoVisible, value);
        }


        private Command _infoCommand;
        public Command InfoCommand
        {
            get => _infoCommand;
            set => SetProperty(ref _infoCommand, value);
        }


        public BetPopupViewModel(string roomName, MatchSnapshot snapshot)
        {
            _snapshot = snapshot;
            _roomName = roomName;
            BetViewModel = _snapshot.Player.BetViewModel;
            BetCommand = new Command<int>(Bet);
            InfoCommand = new Command(_ => ToInfoPopup());
            CheckInfoVisible();
        }

        private async void ToInfoPopup()
        {
            if(_missingNumber is null)
                throw new Exception("Missing number cannot be null");

            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToBetInfoPopup(_missingNumber.Value);
            IsPageEnabled = true;
        }

        private void CheckInfoVisible()
        {
            var cardsCount = _snapshot.Player.StartCardsCount;
            var range = Enumerable.Range(0, cardsCount + 1).Except(_betViewModel.PossibleBets);
            var enumerable = range as int[] ?? range.ToArray();
            
            if (enumerable.Any())
                _missingNumber = enumerable.FirstOrDefault();
            IsInfoVisible = _missingNumber != null;
        }


        private async void Bet(int bet)
        {
            IsPageEnabled = false;

            try
            {
                var query = new RoomQuery<int> { PlayerName = _snapshot.Player.Name, RoomName = _roomName, Data = bet };
                await AppController.RoomsHandler.MakeABet(query);
                await AppController.Navigation.GameNavigation.NotifyBetCompleted();
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }

            IsPageEnabled = true;
        }
    }
}
