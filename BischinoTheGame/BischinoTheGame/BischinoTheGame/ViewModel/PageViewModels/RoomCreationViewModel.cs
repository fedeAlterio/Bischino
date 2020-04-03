using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RoomCreationViewModel : PageViewModel
    {
        private Room _room;
        public Room Room
        {
            get
            {
                CreateRoomCommand.ChangeCanExecute();
                return _room;
            }
            set => SetProperty(ref _room, value);
        }


        private Command _createRoomCommand;
        public Command CreateRoomCommand
        {
            get => _createRoomCommand;
            set => SetProperty(ref _createRoomCommand, value);
        }




        public RoomCreationViewModel()
        {
            var user = AppController.Navigation.RoomNavigation.LoggedPlayer;
            Room = new Room {Host = user.Name};
            CreateRoomCommand = new Command(_=>CreateRoom(), _=>CanCreateRoom());
        }


        private async void CreateRoom()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.RoomsHandler.Create(_room);

                var query = new RoomQuery {PlayerName = Room.Host, RoomName = Room.Name};
                await AppController.RoomsHandler.Join(query);
                await AppController.Navigation.RoomNavigation.NotifyRoomCreated(_room);
            }
            catch (ServerException e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            catch
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, ErrorDefault);
            }

            IsPageEnabled = true;
        }

        private bool CanCreateRoom()
        {
            if (string.IsNullOrWhiteSpace(_room.Name))
                return false;
            if (_room.Name.Any(char.IsWhiteSpace))
                return false;
            if (_room.Name.Length > 16)
                return false;
            if (_room.MinPlayers is null || _room.MaxPlayers is null)
                return false;
            if (_room.MinPlayers < 2 || _room.MinPlayers > 6)
                return false;
            if (_room.MaxPlayers < 2 || _room.MaxPlayers > 6)
                return false;
            if (_room.MinPlayers > _room.MaxPlayers)
                return false;
            return true;
        }
    }
}
