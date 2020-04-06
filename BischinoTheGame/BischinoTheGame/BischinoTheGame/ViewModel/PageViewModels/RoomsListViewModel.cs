using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rooms.Controller;
using Rooms.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RoomsListViewModel : PageViewModel
    {
        public ObservableRangeCollection<Room> Rooms { get; } = new ObservableRangeCollection<Room>();


        private bool _isUpdatingList;
        private bool IsUpdatingList
        {
            get => _isUpdatingList;
            set
            {
                _isUpdatingList = value;
                EndOfListCommand.ChangeCanExecute();
                RefreshCommand.ChangeCanExecute();
            }
        }


        private RoomSearchQuery _query;
        public RoomSearchQuery Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }


        private Room _selectedRoom;
        public Room SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }


        private Command _refreshCommand;
        public Command RefreshCommand
        {
            get => _refreshCommand;
            set => SetProperty(ref _refreshCommand, value);
        }


        private Command _endOfListCommand;
        public Command EndOfListCommand
        {
            get => _endOfListCommand;
            set => SetProperty(ref _endOfListCommand, value);
        }


        private Command<Room> _visualizeRoomCommand;

        public Command<Room> VisualizeRoomCommand
        {
            get => _visualizeRoomCommand;
            set => SetProperty(ref _visualizeRoomCommand, value);
        }


        private Command _showFiltersCommand;
        public Command ShowFiltersCommand
        {
            get => _showFiltersCommand;
            set => SetProperty(ref _showFiltersCommand, value);
        }


        private Command _createRoomCommand;
        public Command CreateRoomCommand
        {
            get => _createRoomCommand;
            set => SetProperty(ref _createRoomCommand, value);
        }


        public RoomsListViewModel()
        {
            Query = new RoomSearchQuery
            {
                Model = new Room(),
                Options = new CollectionQueryOptions { Limit = 4}
            };
            RefreshCommand = new Command(_ => Refresh());
            EndOfListCommand = new Command(_ => EndReached());
            VisualizeRoomCommand = new Command<Room>(OnRoomSelected);
            ShowFiltersCommand = new Command(_ => ShowFilters());
            CreateRoomCommand = new Command(_ => CreateRoom());
        }



        private async void CreateRoom()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ShowRoomCreationPopup();
            IsPageEnabled = true;
        }


        public async void AsyncInitialization()
        {
            await GetRooms();
        }


        private async void ShowFilters()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ToFilterPopup(Query);
            IsPageEnabled = true;
        }


        private async void OnRoomSelected(Room room)
        {
            if (SelectedRoom is null)
                return;

            IsPageEnabled = false;
            try
            {
                var player = AppController.Navigation.RoomNavigation.LoggedPlayer;
                var roomQuery = new RoomQuery { PlayerName = player.Name, RoomName = room.Name };
                await AppController.RoomsHandler.Join(roomQuery);
                await AppController.Navigation.RoomNavigation.NotifyRoomJoined(room);
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
            SelectedRoom = null;
        }


        private async void EndReached()
        {
            await GetRooms();
        }


        private async void Refresh()
        {
            IsBusy = true;
            Query.Options.Skip = 0;
            Rooms.Clear();
            await GetRooms();
            IsBusy = false;
        }


        private async Task GetRooms()
        {
            if (IsUpdatingList)
                return;

            IsUpdatingList = true;
            try
            {
                var result = await AppController.RoomsHandler.GetRooms(Query);
                var rooms = result;
                Query.Options.Skip += rooms.Count;
                foreach (var room in rooms)
                    Rooms.Add(room);
            }
            catch (ServerException e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            catch (Exception)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, ErrorDefault);
            }

            IsUpdatingList = false;
        }
    }
}
