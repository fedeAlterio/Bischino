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


        private RulesViewModel _rulesViewModel;
        public RulesViewModel RulesViewModel
        {
            get => _rulesViewModel;
            set => SetProperty(ref _rulesViewModel, value);
        }



        public CoreTabbedViewModel(RoomsListViewModel roomListViewModel, SettingsViewModel settingsViewModel, RulesViewModel rulesViewModel)
        {
            (RoomListViewModel, SettingsViewModel, RulesViewModel) = (roomListViewModel, settingsViewModel, rulesViewModel);
        }
    }
}
