using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RoomsListViewModel : PageViewModel
    {
        public ObservableRangeCollection<Room> Rooms { get; } = new ObservableRangeCollection<Room>();

        // Initialization
        public RoomsListViewModel()
        {
            Query = new RoomSearchQuery
            {
                Model = new Room(),
                Options = new CollectionQueryOptions { Limit = 4 }
            };
            RefreshCommand = NewCommand(Refresh);
            EndOfListCommand = NewCommand(EndReached);
            VisualizeRoomCommand = NewCommand<Room>(OnRoomSelected);
            ShowFiltersCommand = NewCommand(ShowFilters);
            CreateRoomCommand = NewCommand(CreateRoom);
            ToPrivateRoomLockerCommand = NewCommand(ToPrivateRoomLocker);
        }

        public async Task AsyncInitialization()
        {
            await GetRooms();
        }


        // Commands
        public IAsyncCommand RefreshCommand { get; }
        public IAsyncCommand EndOfListCommand { get; }
        public IAsyncCommand<Room> VisualizeRoomCommand { get; }
        public IAsyncCommand ShowFiltersCommand { get; }
        public IAsyncCommand CreateRoomCommand { get; }
        public IAsyncCommand ToPrivateRoomLockerCommand { get; }



        // Properties
        private bool _isUpdatingList;
        private bool IsUpdatingList
        {
            get => _isUpdatingList;
            set
            {
                _isUpdatingList = value;
                EndOfListCommand.RaiseCanExecuteChanged();
                RefreshCommand.RaiseCanExecuteChanged();
            }
        }

        public RoomSearchQuery Query { get; }


        private Room _selectedRoom;
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }


        
        // Commands Handlers
        private async Task ToPrivateRoomLocker()
        {
            await AppController.Navigation.GameNavigation.ToPrivateRoomLocker();
        }


        private async Task CreateRoom()
        {
           await AppController.Navigation.GameNavigation.ShowRoomCreationPopup();
        }


        private async Task ShowFilters()
        {
            await AppController.Navigation.GameNavigation.ToFilterPopup(Query);
        }


        private async Task OnRoomSelected(Room room)
        {
            if (SelectedRoom is null)
                return;

            var player = AppController.Navigation.GameNavigation.LoggedPlayer;
            var roomQuery = new RoomQuery { PlayerName = player.Name, RoomName = room.Name };
            await AppController.GameHandler.Join(roomQuery);
            await AppController.Navigation.GameNavigation.NotifyRoomJoined(room);
            SelectedRoom = null;
        }


        private async Task EndReached() => await GetRooms();


        private async Task Refresh()
        {
            IsBusy = true;
            Query.Options.Skip = 0;
            Rooms.Clear();
            await GetRooms();
            IsBusy = false;
        }



        // Helpers
        private async Task GetRooms()
        {
            if (IsUpdatingList)
                return;

            IsUpdatingList = true;
            try
            {
                var result = await AppController.GameHandler.GetRooms(Query);
                var rooms = result;
                Query.Options.Skip += rooms.Count;
                foreach (var room in rooms)
                    Rooms.Add(room);
            }
            catch
            {
                throw;
            }
            finally
            {
                IsUpdatingList = false;
            }
        }
    }
}
