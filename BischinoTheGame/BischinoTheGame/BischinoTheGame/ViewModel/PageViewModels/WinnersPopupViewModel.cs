using System;
using System.Collections.Generic;
using System.Text;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class WinnersPopupViewModel : PageViewModel
    {
        private MatchSnapshot _matchSnapshot;
        public MatchSnapshot MatchSnapshot
        {
            get => _matchSnapshot;
            set => SetProperty(ref _matchSnapshot, value);
        }


        private Command _okCommand;
        public Command OkCommand
        {
            get => _okCommand;
            set => SetProperty(ref _okCommand, value);
        }


        public WinnersPopupViewModel(MatchSnapshot matchSnapshot)
        {
            _matchSnapshot = matchSnapshot;
            OkCommand = new Command(_ => Ok());
        }

        private async void Ok()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.BackToRoomList();
            IsPageEnabled = true;
        }
    }
}
