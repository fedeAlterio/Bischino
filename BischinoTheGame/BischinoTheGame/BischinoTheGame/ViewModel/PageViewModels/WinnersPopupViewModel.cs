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
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.StartChronology();
            IsPageEnabled = true;
        }

        private async Task Ok()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.BackToRoomList(true);
            IsPageEnabled = true;
        }
    }
}
