using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class LastPhasePopupViewModel : PageViewModel
    {
        private readonly string _roomName;
        private readonly string _playerName;


        // Initialization
        public LastPhasePopupViewModel(string roomName, string playerName, LastPhaseViewModel lastPhaseVM)
        {
            (_roomName, _playerName, LastPhaseViewModel) = (roomName, playerName, lastPhaseVM);

            WinBetCommand = NewCommand(async () => await NotifyBet(true));
            LoseBetCommand = NewCommand(async () => await NotifyBet(false));
        }

        // Commands
        public IAsyncCommand WinBetCommand { get; }
        public IAsyncCommand LoseBetCommand { get; }



        // Properties
        public LastPhaseViewModel LastPhaseViewModel { get; }

        private async Task NotifyBet(bool win)
        {
            var roomQuery = new RoomQuery<int> {RoomName = _roomName, PlayerName = _playerName, Data = win ? 1 : 0};
            await AppController.GameHandler.MakeABet(roomQuery);
            await AppController.Navigation.GameNavigation.NotifyLastPhaseCompleted();
        }
    }
}
