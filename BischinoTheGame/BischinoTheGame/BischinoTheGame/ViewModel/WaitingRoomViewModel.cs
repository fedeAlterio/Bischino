using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel
{
    public class WaitingRoomViewModel : PageViewModel
    {
        private Player _player;
        private RoomQuery _roomQuery;


        private Room _room;
        public Room Room
        {
            get => _room;
            set => SetProperty(ref _room, value);
        }

        public ObservableCollection<string> JoinedPlayers { get; set; } = new ObservableCollection<string>();
        public bool IsMatchStarted { get; set; } = false;


        private Command _startMatchCommand;
        public Command StartMatchCommand
        {
            get => _startMatchCommand;
            set => SetProperty(ref _startMatchCommand, value);
        }


        private Command _unJoinCommand;
        public Command UnJoinCommand
        {
            get => _unJoinCommand;
            set => SetProperty(ref _unJoinCommand, value);
        }


        private bool _isHost;

        public bool IsHost
        {
            get => _isHost;
            set => SetProperty(ref _isHost, value);
        }


        public WaitingRoomViewModel(Room room)
        {
            Room = room;
            _player = AppController.Navigation.RoomNavigation.LoggedPlayer;
            _roomQuery = new RoomQuery {PlayerName = _player.Name, RoomName = _room.Name};
            StartMatchCommand = new Command(_ => StartMatch(), _ => CanStart());
            UnJoinCommand = new Command(_ => UnJoin());
            StartPolling();
        }

        private async void UnJoin()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.RoomsHandler.UnJoin(_roomQuery);
                await AppController.Navigation.RoomNavigation.BackToRoomList();
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            IsPageEnabled = true;
        }

        private bool CanStart()
        {
            if (JoinedPlayers.Count > _room.MinPlayers)
                return true;
            return true;// false;
        }

        private async void StartMatch()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.RoomsHandler.Start(_room.Name);
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }

            IsPageEnabled = true;
        }


        private async void StartPolling()
        {
            while (!IsMatchStarted)
            {
                try
                {
                    IsMatchStarted = await AppController.RoomsHandler.IsMatchStarted(_room.Name);
                    await LoadPlayers();
                    StartMatchCommand.ChangeCanExecute();
                    await Task.Delay(500);
                }
                catch { }
            }

            OnMatchStarted();
        }

        private async void OnMatchStarted()
        {
            await AppController.Navigation.RoomNavigation.NotifyMatchStarted(_room);
        }

        private async Task LoadPlayers()
        {
            var joinedPlayers = await AppController.RoomsHandler.GetJoinedPLayers(_roomQuery);
            JoinedPlayers.Clear();
            if (_player.Name == joinedPlayers[0])
                IsHost = true;
            foreach(var player in joinedPlayers)
                JoinedPlayers.Add(player);
        }
    }
}
