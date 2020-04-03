using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
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
        private CancellationTokenSource _pollTokenSource;
        private Task _pollingTask;

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


        private bool _canStart;
        public bool CanStart
        {
            get => _canStart;
            set => SetProperty(ref _canStart, value);
        }


        public WaitingRoomViewModel(Room room)
        {
            Room = room;
            _player = AppController.Navigation.RoomNavigation.LoggedPlayer;
            _roomQuery = new RoomQuery {PlayerName = _player.Name, RoomName = _room.Name};
            StartMatchCommand = new Command(_ => StartMatch(), _ => CanStartCommand());
            UnJoinCommand = new Command(_ => UnJoin());
            _pollTokenSource = new CancellationTokenSource();
            _pollingTask = PollingAsync(_pollTokenSource.Token);
        }

        private async void UnJoin()
        {
            IsPageEnabled = false;
            try
            {
                _pollTokenSource.Cancel();
                await _pollingTask;
                await AppController.RoomsHandler.UnJoin(_roomQuery);
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            finally
            {
                await AppController.Navigation.RoomNavigation.BackToRoomList();
            }
            IsPageEnabled = true;
        }


        private bool CanStartCommand() => CanStart = IsHost;// && JoinedPlayers.Count > _room.MinPlayers;


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


        private async Task PollingAsync(CancellationToken token)
        {
            while (!IsMatchStarted && !token.IsCancellationRequested)
            {
                try
                {
                    await LoadPlayers();
                    StartMatchCommand.ChangeCanExecute();
                    IsMatchStarted = await AppController.RoomsHandler.IsMatchStarted(_room.Name);
                    await Task.Delay(500, token);
                }
                catch (Exception)
                {
                    
                }
            }
            if(IsMatchStarted)
                    OnMatchStarted();
        }

        private async void OnMatchStarted()
        {
            await AppController.Navigation.RoomNavigation.NotifyMatchStarted(_room);
        }

        private async Task LoadPlayers()
        {
            try
            {
                var joinedPlayers = await AppController.RoomsHandler.GetJoinedPLayers(_roomQuery);
                JoinedPlayers.Clear();
                IsHost = _player.Name == joinedPlayers[0];
                foreach (var player in joinedPlayers)
                    JoinedPlayers.Add(player);
            }
            catch (Exception e)
            {
            }
    
        }
    }
}
