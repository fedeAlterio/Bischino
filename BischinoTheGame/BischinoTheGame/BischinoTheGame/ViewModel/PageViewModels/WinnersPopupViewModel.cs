using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
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



        private Command _toChronologyCommand;
        public Command ToChronologyCommand
        {
            get => _toChronologyCommand;
            set => SetProperty(ref _toChronologyCommand, value);
        }



        public WinnersPopupViewModel(MatchSnapshot matchSnapshot)
        {
            _matchSnapshot = matchSnapshot;
            OkCommand = new Command(_ => Ok());
            ToChronologyCommand = new Command(_ => StartChronology());
        }

        private async void StartChronology()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.StartChronology();
            IsPageEnabled = true;
        }

        private async void Ok()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.BackToRoomList(true);
            IsPageEnabled = true;
        }
    }
}
