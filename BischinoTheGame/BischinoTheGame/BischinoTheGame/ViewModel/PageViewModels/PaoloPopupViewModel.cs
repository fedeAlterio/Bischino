using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller.Communication.Queries;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class PaoloPopupViewModel : PageViewModel
    {
        private readonly string _roomName;
        private readonly string _playerName;
        private Command _maxCommand;
        public Command MaxCommand
        {
            get => _maxCommand;
            set => SetProperty(ref _maxCommand, value);
        }


        private Command _minCommand;
        public Command MinCommand
        {
            get => _minCommand;
            set => SetProperty(ref _minCommand, value);
        }


        public PaoloPopupViewModel(string roomName, string playerName)
        {
            _roomName = roomName;
            _playerName = playerName;
            MaxCommand = new Command(_=>Notify(true));
            MinCommand = new Command(_=>Notify(false));
        }

        private async void Notify(bool isMax)
        {
            IsPageEnabled = false;
            try
            {
                var query = new RoomQuery<bool> {RoomName = _roomName, PlayerName = _playerName, Data = isMax};
                await AppController.RoomsHandler.DropPaolo(query);
                await AppController.Navigation.RoomNavigation.NotifyPaoloSent();
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }

            IsPageEnabled = true;
        }
    }
}
