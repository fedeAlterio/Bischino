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
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel
{
    public class WaitingRoomViewModel : PageViewModel
    {
        private readonly Player _player;
        private readonly RoomQuery _roomQuery;
        private readonly CancellationTokenSource _pollTokenSource;
        private readonly Task _pollingTask;

        
        public bool IsUnjoined { get; set; }
        public bool IsMatchStarted { get; private set; }



        public ObservableCollection<JoinedPlayer> JoinedPlayers { get; } = new ObservableCollection<JoinedPlayer>();


        private WaitingRoomInfo _waitingRoomInfo;
        public WaitingRoomInfo WaitingRoomInfo
        {
            get => _waitingRoomInfo;
            set => SetProperty(ref _waitingRoomInfo, value);
        }



        public Command AddBotCommand {get;}
        private async void AddBot()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.GameHandler.AddBot(_roomQuery);
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

        private bool CanAddBot() => _waitingRoomInfo?.NotBotPlayers != null && WaitingRoomInfo.BotCounter + WaitingRoomInfo.NotBotPlayers.Count < _room.MaxPlayers;



        public Command RemoveABotCommand { get; } 
        private async void RemoveABot()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.GameHandler.RemoveABot(_roomQuery);
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
        private bool CanRemoveABot() => _waitingRoomInfo != null && _waitingRoomInfo.BotCounter > 0;
        


        private Room _room;
        public Room Room
        {
            get => _room;
            set => SetProperty(ref _room, value);
        }



        public Command StartMatchCommand { get; } 
        private async void StartMatch()
        {
            IsPageEnabled = false;
            try
            {
                await AppController.GameHandler.Start(_room.Name);
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
                IsPageEnabled = true;
            }
        }
        private bool CanStartCommandCheck() => CanStart = IsHost && JoinedPlayers.Count >= _room.MinPlayers;



        public Command UnJoinCommand { get; }
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




        public WaitingRoomViewModel(Room room)
        {
            Room = room;
            _player = AppController.Navigation.GameNavigation.LoggedPlayer;
            _roomQuery = new RoomQuery {PlayerName = _player.Name, RoomName = _room.Name};
            _pollTokenSource = new CancellationTokenSource();
            _pollingTask = PollingAsync(_pollTokenSource.Token);

            RemoveABotCommand = new Command(_ => RemoveABot(), _ => CanRemoveABot());
            AddBotCommand = new Command(_ => AddBot(), _ => CanAddBot());
            UnJoinCommand = new Command(async _ => await UnJoin());
            StartMatchCommand = new Command(_ => StartMatch(), _ => CanStartCommandCheck());
        }



        private async Task PollingAsync(CancellationToken token)
        {
            while (!IsMatchStarted && !token.IsCancellationRequested)
            {
                try
                {
                    await LoadPlayers();
                    IsMatchStarted = await AppController.GameHandler.IsMatchStarted(_room.Name);
                    await Task.Delay(500, token);
                }
                catch (Exception e)
                {

                }
            }
            if(IsMatchStarted)
                    OnMatchStarted();
        }

        private async void OnMatchStarted()
        {
            for(var i=0; i < 3; i++)
                try
                {
                    var roomInfo = await AppController.GameHandler.GetGameInfo(_roomQuery);
                    await AppController.Navigation.GameNavigation.NotifyMatchStarted(_room, roomInfo);
                    return;
                }
                catch (Exception e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, "Impossible to communicate with the server, try again?");
                }
            await AppController.Navigation.DisplayAlert(ErrorTitle, "Impossible to communicate with the server, going back to room list?");
            await AppController.Navigation.GameNavigation.BackToRoomList();
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

                JoinedPlayers.Clear();
                WaitingRoomInfo = waitingRoomInfo;
                IsHost = _player.Name == _waitingRoomInfo.NotBotPlayers[0];

                UpdateJoinedPlayers();
                UpdateBots();

                AddBotCommand.ChangeCanExecute();
                RemoveABotCommand.ChangeCanExecute();
                StartMatchCommand.ChangeCanExecute();
            }
            catch (Exception e)
            {
            }
        }

        private void UpdateJoinedPlayers()
        {
            foreach (var player in _waitingRoomInfo.NotBotPlayers)
                JoinedPlayers.Add(new JoinedPlayer
                {
                    Name = player,
                    IconName = player == _waitingRoomInfo.NotBotPlayers[0] ? "host_user" : "standard_user"
                });
        }

        private void UpdateBots()
        {
            for (int i = 0; i < _waitingRoomInfo.BotCounter; i++)
                JoinedPlayers.Add(new JoinedPlayer
                {
                    Name = $"bot{i}",
                    IconName = "bot"
                });
        }
    }
}
