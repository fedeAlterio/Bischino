using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel
{
    public class WaitingRoomViewModel : PageViewModel
    {
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


        private bool _isHost;
        public bool IsHost
        {
            get => _isHost;
            set => SetProperty(ref _isHost, value);
        }


        public WaitingRoomViewModel(Room room, bool isHost)
        {
            Room = room;
            _isHost = isHost;
            StartPolling();
            if(isHost)
                StartMatchCommand = new Command(_ => StartMatch(), _ => CanStart());
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
            var joinedPlayers = await AppController.RoomsHandler.GetJoinedPLayers(_room.Name);
            JoinedPlayers.Clear();
            foreach(var player in joinedPlayers)
                JoinedPlayers.Add(player);
        }
    }
}
