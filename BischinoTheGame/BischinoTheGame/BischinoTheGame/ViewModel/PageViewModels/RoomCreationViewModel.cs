using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.View.ViewElements;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RoomCreationViewModel : PageViewModel
    {
        private static readonly Color NotSelectedColor = Color.FromHex("#303f9f");
        private static readonly Color SelectedColor = Color.DarkRed;


        // Initialization
        public RoomCreationViewModel()
        {
            var user = AppController.Navigation.GameNavigation.LoggedPlayer;
            Room = new() { Host = user.Name };
            CreateRoomCommand = NewCommand(CreateRoom, CanCreateRoom);
            var minColors = from val in new List<int> { 2, 3, 4, 5, 6 }
                            select new ColorWrapper<int> { Color = NotSelectedColor, Model = val };

            var maxColors = from val in new List<int> { 2, 3, 4, 5, 6 }
                            select new ColorWrapper<int> { Color = NotSelectedColor, Model = val };

            _possibleMinPlayers = new(minColors);
            PossibleMinPlayers = new (_possibleMinPlayers);

            _possibleMaxPlayers = new(maxColors);
            PossibleMaxPlayers = new (_possibleMaxPlayers);
        }


        // Commands
        public IAsyncCommand CreateRoomCommand { get; }


        
        // Properties
        private Room _room;
        public Room Room
        {
            get
            {
                CreateRoomCommand.RaiseCanExecuteChanged();
                return _room;
            }
            set => SetProperty(ref _room, value);
        }


        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }



        private ObservableCollection<ColorWrapper<int>> _possibleMinPlayers;
        public ReadOnlyObservableCollection<ColorWrapper<int>> PossibleMinPlayers { get; }


        private ObservableCollection<ColorWrapper<int>> _possibleMaxPlayers;
        public ReadOnlyObservableCollection<ColorWrapper<int>> PossibleMaxPlayers { get; }       


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
                CreateRoomCommand.RaiseCanExecuteChanged();

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
                CreateRoomCommand.RaiseCanExecuteChanged();

                SetProperty(ref _selectedMaxPlayer, value);
            }
        }


        // Commands Handlers
        private async Task CreateRoom()
        {
            Room = await AppController.GameHandler.Create(_room);

            var query = new RoomQuery {PlayerName = Room.Host, RoomName = Room.Name};
            await AppController.GameHandler.Join(query);
            await AppController.Navigation.GameNavigation.NotifyRoomCreated(_room);
        }

        private bool CanCreateRoom()
        {
            // if is not null, there is an error. (empty is stil an error)
            ErrorMessage = Room switch
            {
                var x when string.IsNullOrWhiteSpace(x.Name) => string.Empty,
                var x when x.Name.Any(char.IsWhiteSpace) => "Make sure there are not spaces",
                { Name: {Length: > 16 } } => "The name can be only 16 character long",
                { MinPlayers: null } or { MaxPlayers: null } => string.Empty,                
                { MinPlayers: < 2 or > 6 } => "The field min players should be greater or equal to 2, and less or equal to 6",
                { MaxPlayers: < 2 or > 6 } => "The field max players should be greater or equal to 2, and less or equal to 6",
                var x when x.MinPlayers > x.MaxPlayers => "The field max players should be greater than min players",
                _ => null
            };
            return ErrorMessage is null;
        }
    }
}
