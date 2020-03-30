using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller.Communication.Queries;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class BetPopupViewModel : PageViewModel
    {
        private readonly string _roomName;
        private readonly string _playerName;


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


        public BetPopupViewModel(string roomName, string playerName, BetViewModel vm)
        {
            _roomName = roomName;
            _playerName = playerName;
            _betViewModel = vm;
            BetCommand = new Command<int>(Bet);
        }



        private async void Bet(int bet)
        {
            IsPageEnabled = false;

            try
            {
                var query = new RoomQuery<int> { PlayerName = _playerName, RoomName = _roomName, Data = bet };
                await AppController.RoomsHandler.MakeABet(query);
                await AppController.Navigation.RoomNavigation.NotifyBetCompleted();
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }

            IsPageEnabled = true;
        }
    }
}
