using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class CoreTabbedViewModel : PageViewModel
    {
        public CoreTabbedViewModel(RoomsListViewModel roomListViewModel, SettingsViewModel settingsViewModel, RulesViewModel rulesViewModel)
        {
            (RoomListViewModel, SettingsViewModel, RulesViewModel) = (roomListViewModel, settingsViewModel, rulesViewModel);
        }


        // Properties

        public RoomsListViewModel RoomListViewModel { get; }
        public SettingsViewModel SettingsViewModel { get; }
        public RulesViewModel RulesViewModel { get; }
    }
}
