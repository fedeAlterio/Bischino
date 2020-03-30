using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class LastPhasePopupViewModel : PageViewModel
    {
        private readonly string _roomName;
        private readonly string _playerName;

        private LastPhaseViewModel _lastPhaseViewModel;
        public LastPhaseViewModel LastPhaseViewModel
        {
            get => _lastPhaseViewModel;
            set => SetProperty(ref _lastPhaseViewModel, value);
        }


        private Command _winBetCommand;
        public Command WinBetCommand
        {
            get => _winBetCommand;
            set => SetProperty(ref _winBetCommand, value);
        }


        private Command _loseBetCommand;
        public Command LoseBetCommand
        {
            get => _loseBetCommand;
            set => SetProperty(ref _loseBetCommand, value);
        }



        public LastPhasePopupViewModel(string roomName, string playerName, LastPhaseViewModel lastPhaseVM)
        {
            (_roomName, _playerName, LastPhaseViewModel) = (roomName, playerName, lastPhaseVM);

            WinBetCommand = new Command(_ => NotifyBet(true));
            LoseBetCommand = new Command(_ => NotifyBet(false));
        }

        private async void NotifyBet(bool win)
        {
            IsPageEnabled = false;
            try
            {
                var roomQuery = new RoomQuery<int> {RoomName = _roomName, PlayerName = _playerName, Data = win ? 1 : 0};
                await AppController.RoomsHandler.MakeABet(roomQuery);
                await AppController.Navigation.RoomNavigation.NotifyLastPhaseCompleted();
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            IsPageEnabled = true;
        }
    }
}
