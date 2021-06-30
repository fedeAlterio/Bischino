using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel
{
    public class WaitingRoomViewModel : PageViewModel
    {
        private readonly Player _player;
        private readonly RoomQuery _roomQuery;
        private readonly CancellationTokenSource _pollTokenSource;
        private readonly Task _pollingTask;




        // Initialization
        public WaitingRoomViewModel(Room room)
        {
            Room = room;
            _player = AppController.Navigation.GameNavigation.LoggedPlayer;
            _roomQuery = new RoomQuery { PlayerName = _player.Name, RoomName = Room.Name };
            _pollTokenSource = new CancellationTokenSource();
            _pollingTask = PollingAsync(_pollTokenSource.Token);

            RemoveABotCommand = NewCommand(RemoveABot, CanRemoveABot);
            AddBotCommand = NewCommand(AddBot, CanAddBot);
            UnJoinCommand = NewCommand(UnJoin);
            StartMatchCommand = NewCommand(StartMatch, CanStartCommandCheck);

            JoinedPlayers = new(_joinedPlayers);
        }


        // Commands
        public IAsyncCommand RemoveABotCommand { get; }
        public IAsyncCommand AddBotCommand { get; }
        public IAsyncCommand UnJoinCommand { get; }
        public IAsyncCommand StartMatchCommand { get; }


        
        // Properties
        public bool IsUnjoined { get; set; }
        public bool IsMatchStarted { get; private set; }


        private readonly ObservableCollection<JoinedPlayer> _joinedPlayers = new();
        public ReadOnlyObservableCollection<JoinedPlayer> JoinedPlayers { get; }


        private WaitingRoomInfo _waitingRoomInfo;
        public WaitingRoomInfo WaitingRoomInfo
        {
            get => _waitingRoomInfo;
            set => SetProperty(ref _waitingRoomInfo, value);
        }


        public Room Room { get; }

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



        public string RoomName => $"{Room.Name} ({Room.RoomNumber})";



        // Commands Handlers
        private Task AddBot() => AppController.GameHandler.AddBot(_roomQuery);
        private bool CanAddBot() => _waitingRoomInfo?.NotBotPlayers != null && WaitingRoomInfo.BotCounter + WaitingRoomInfo.NotBotPlayers.Count < Room.MaxPlayers;

        private Task RemoveABot() => AppController.GameHandler.RemoveABot(_roomQuery);
        private bool CanRemoveABot() => _waitingRoomInfo != null && _waitingRoomInfo.BotCounter > 0;
        

        private Task StartMatch() => AppController.GameHandler.Start(Room.Name);
        private bool CanStartCommandCheck() => CanStart = IsHost && JoinedPlayers.Count >= Room.MinPlayers;



        public async Task UnJoin()
        {
            if (IsUnjoined)
                return;

            IsPageEnabled = false;
            try
            {
                _pollTokenSource.Cancel();
                await _pollingTask;
                await AppController.GameHandler.UnJoin(_roomQuery);
            }
            catch (Exception e)
            {
                //await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            finally
            {
                IsUnjoined = true;
                await AppController.Navigation.GameNavigation.BackToRoomList();
            }
            IsPageEnabled = true;
        }



        // Events
        private async Task OnMatchStarted()
        {
            for(var i=0; i < 3; i++)
                try
                {
                    var roomInfo = await AppController.GameHandler.GetGameInfo(_roomQuery);
                    await AppController.Navigation.GameNavigation.NotifyMatchStarted(Room, roomInfo);
                    return;
                }
                catch (Exception e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, "Impossible to communicate with the server, try again?");
                }
            await AppController.Navigation.DisplayAlert(ErrorTitle, "Impossible to communicate with the server, going back to room list?");
            await AppController.Navigation.GameNavigation.BackToRoomList();
        }



        // Helpers
        private async Task PollingAsync(CancellationToken token)
        {
            while (!IsMatchStarted && !token.IsCancellationRequested)
            {
                try
                {
                    await LoadPlayers();
                    IsMatchStarted = await AppController.GameHandler.IsMatchStarted(Room.Name);
                    await Task.Delay(500, token);
                }
                catch (Exception e)
                {

                }
            }
            if (IsMatchStarted)
                await OnMatchStarted();
        }


        private async Task LoadPlayers()
        {
            try
            {
                var waitingRoomInfo = await AppController.GameHandler.GetWaitingRoomInfo(_roomQuery);
                if (_waitingRoomInfo != null && !waitingRoomInfo.NotBotPlayers.Contains(_player.Name))
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, "Connection lost");
                    await UnJoin();
                }

                _joinedPlayers.Clear();
                WaitingRoomInfo = waitingRoomInfo;
                IsHost = _player.Name == _waitingRoomInfo.NotBotPlayers[0];

                UpdateJoinedPlayers();
                UpdateBots();

                AddBotCommand.RaiseCanExecuteChanged();
                RemoveABotCommand.RaiseCanExecuteChanged();
                StartMatchCommand.RaiseCanExecuteChanged();
            }
            catch { }
        }

        private void UpdateJoinedPlayers()
        {
            foreach (var player in _waitingRoomInfo.NotBotPlayers)
                _joinedPlayers.Add(new JoinedPlayer
                {
                    Name = player,
                    IconName = player == _waitingRoomInfo.NotBotPlayers[0] ? "host_user" : "standard_user"
                });
        }

        private void UpdateBots()
        {
            for (int i = 0; i < _waitingRoomInfo.BotCounter; i++)
                _joinedPlayers.Add(new JoinedPlayer
                {
                    Name = $"bot{i}",
                    IconName = "bot"
                });
        }
    }
}
