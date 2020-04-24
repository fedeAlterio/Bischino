using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.View.ViewElements;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RoomCreationViewModel : PageViewModel
    {
        private static readonly Color NotSelectedColor = Color.FromHex("#303f9f");
        private static readonly Color SelectedColor = Color.DarkRed;

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



        private ObservableCollection<ColorWrapper<int>> _possibleMinPlayers;
        public ObservableCollection<ColorWrapper<int>> PossibleMinPlayers
        {
            get => _possibleMinPlayers;
            set => SetProperty(ref _possibleMinPlayers, value);
        }



        private ObservableCollection<ColorWrapper<int>> _possibleMaxPlayers;
        public ObservableCollection<ColorWrapper<int>> PossibleMaxPlayers
        {
            get => _possibleMaxPlayers;
            set => SetProperty(ref _possibleMaxPlayers, value);
        }




        private ColorWrapper<int> _selectedMinPlayer;
        public ColorWrapper<int> SelectedMinPlayer
        {
            get => _selectedMinPlayer;
            set
            {
                foreach (var colorWrapper in _possibleMinPlayers)
                    colorWrapper.Color = NotSelectedColor;
                
                value.Color = SelectedColor;
                
                _room.MinPlayers = value.Model;
                _selectedMinPlayer = null;
                CreateRoomCommand.ChangeCanExecute();

                SetProperty(ref _selectedMinPlayer, value);
            }
        }




        private ColorWrapper<int> _selectedMaxPlayer;
        public ColorWrapper<int> SelectedMaxPlayer
        {
            get => _selectedMaxPlayer;
            set
            {
                foreach (var colorWrapper in _possibleMaxPlayers)
                    colorWrapper.Color = NotSelectedColor;
                value.Color = SelectedColor;
                
                _room.MaxPlayers = value.Model;
                _selectedMaxPlayer = null;
                CreateRoomCommand.ChangeCanExecute();

                SetProperty(ref _selectedMaxPlayer, value);
            }
        }



        public RoomCreationViewModel()
        {
            var user = AppController.Navigation.GameNavigation.LoggedPlayer;
            Room = new Room {Host = user.Name};
            CreateRoomCommand = new Command(_=>CreateRoom(), _=>CanCreateRoom());
            var minColors = from val in new List<int> {2, 3, 4, 5, 6}
                select new ColorWrapper<int>{ Color = NotSelectedColor, Model = val};

            var maxColors = from val in new List<int> { 2, 3, 4, 5, 6 }
                select new ColorWrapper<int> { Color = NotSelectedColor, Model = val };

            PossibleMinPlayers = new ObservableCollection<ColorWrapper<int>>(minColors);
            PossibleMaxPlayers = new ObservableCollection<ColorWrapper<int>>(maxColors);
        }



        private async void CreateRoom()
        {
            IsPageEnabled = false;
            try
            {
                Room = await AppController.GameHandler.Create(_room);

                var query = new RoomQuery {PlayerName = Room.Host, RoomName = Room.Name};
                await AppController.GameHandler.Join(query);
                await AppController.Navigation.GameNavigation.NotifyRoomCreated(_room);
            }
            catch (ServerException e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            catch ( Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, ErrorDefault);
            }

            IsPageEnabled = true;
        }

        private bool CanCreateRoom()
        {
            ErrorMessage = string.Empty;
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
