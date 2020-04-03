using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class CoreTabbedViewModel : PageViewModel
    {
        private RoomsListViewModel _roomListViewModel;
        public RoomsListViewModel RoomListViewModel
        {
            get => _roomListViewModel;
            set => SetProperty(ref _roomListViewModel, value);
        }


        private SettingsViewModel _settingsViewModel;
        public SettingsViewModel SettingsViewModel
        {
            get => _settingsViewModel;
            set => SetProperty(ref _settingsViewModel, value);
        }


        public CoreTabbedViewModel(RoomsListViewModel roomListViewModel, SettingsViewModel settingsViewModel)
        {
            (RoomListViewModel, SettingsViewModel) = (roomListViewModel, settingsViewModel);
        }
    }
}
