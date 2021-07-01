using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class WinnersPopupViewModel : PageViewModel
    {
        // Initialization
        public WinnersPopupViewModel(MatchSnapshot matchSnapshot)
        {
            MatchSnapshot = matchSnapshot;
            OkCommand = NewCommand(Ok);
            ToChronologyCommand = NewCommand(StartChronology);
        }


        // Commands
        public IAsyncCommand OkCommand { get; }
        public IAsyncCommand ToChronologyCommand { get; }


        // Properties
        public MatchSnapshot MatchSnapshot { get; }



        // Commands Handlers
        private async Task StartChronology()
        {
            await AppController.Navigation.GameNavigation.StartChronology();
        }

        private async Task Ok()
        {
            await AppController.Navigation.GameNavigation.BackToRoomList(true);
        }
    }
}
