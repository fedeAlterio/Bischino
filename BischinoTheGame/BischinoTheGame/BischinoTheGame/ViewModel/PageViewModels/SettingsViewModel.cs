using System;
using System.Collections.Generic;
using System.Text;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class SettingsViewModel : PageViewModel
    {
        private Command _toDeckSelectionCommand;
        public Command ToDeckSelectionCommand
        {
            get => _toDeckSelectionCommand;
            set => SetProperty(ref _toDeckSelectionCommand, value);
        }


        public SettingsViewModel()
        {
            ToDeckSelectionCommand = new Command(_ => ToDeckSelection());
        }

        private async void ToDeckSelection()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ToDeckSelection();
            IsPageEnabled = true;
        }
    }
}
