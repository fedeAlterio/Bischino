using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BischinoTheGame.Controller;
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


        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }


        private ObservableCollection<int> _possibleMinPlayers;
        public ObservableCollection<int> PossibleMinPlayers
        {
            get => _possibleMinPlayers;
            set => SetProperty(ref _possibleMinPlayers, value);
        }


        private ObservableCollection<int> _possibleMaxPlayers;
        public ObservableCollection<int> PossibleMaxPlayers
        {
            get => _possibleMaxPlayers;
            set => SetProperty(ref _possibleMaxPlayers, value);
        }



        public RoomCreationViewModel()
        {
            var user = AppController.Navigation.GameNavigation.LoggedPlayer;
            Room = new Room {Host = user.Name};
            CreateRoomCommand = new Command(_=>CreateRoom(), _=>CanCreateRoom());
            PossibleMinPlayers = new ObservableCollection<int> {2, 3, 4, 5, 6};
            PossibleMaxPlayers = new ObservableCollection<int>{2,3,4,5,6};
        }



        private async void CreateRoom()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.RoomsHandler.Create(_room);

                var query = new RoomQuery {PlayerName = Room.Host, RoomName = Room.Name};
                await AppController.RoomsHandler.Join(query);
                await AppController.Navigation.GameNavigation.NotifyRoomCreated(_room);
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
            {
                ErrorMessage = string.Empty;
                return false;
            }
            if (_room.Name.Any(char.IsWhiteSpace))
            {
                ErrorMessage = "Make sure there are not spaces";
                return false;
            }
            if (_room.Name.Length > 16)
            {
                ErrorMessage = "The name can be only 16 character long";
                return false;
            }
            if (_room.MinPlayers is null || _room.MaxPlayers is null)
                return false;
            if (_room.MinPlayers < 2 || _room.MinPlayers > 6)
            {
                ErrorMessage = "The field min players should be greater or equal to 2, and less or equal to 6";
                return false;
            }
            if (_room.MaxPlayers < 2 || _room.MaxPlayers > 6)
            {
                ErrorMessage = "The field max players should be greater or equal to 2, and less or equal to 6";
                return false;
            }
            if (_room.MinPlayers > _room.MaxPlayers)
            {
                ErrorMessage = "The field max players should be greater than min players";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}
