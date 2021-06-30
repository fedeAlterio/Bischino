using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Queries;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class PaoloPopupViewModel : PageViewModel
    {
        private readonly string _roomName;
        private readonly string _playerName;


        // Initialization
        public PaoloPopupViewModel(string roomName, string playerName)
        {
            _roomName = roomName;
            _playerName = playerName;
            MaxCommand = NewCommand(async () => await Notify(true));
            MinCommand = new Command(async () => await Notify(false));
        }

        // Commands
        public IAsyncCommand MaxCommand { get; }
        public Command MinCommand { get; }



        // Commands Handlers
        private async Task Notify(bool isMax)
        {
            var query = new RoomQuery<bool> {RoomName = _roomName, PlayerName = _playerName, Data = isMax};
            await AppController.GameHandler.DropPaolo(query);
            await AppController.Navigation.GameNavigation.NotifyPaoloSent();
        }
    }
}
