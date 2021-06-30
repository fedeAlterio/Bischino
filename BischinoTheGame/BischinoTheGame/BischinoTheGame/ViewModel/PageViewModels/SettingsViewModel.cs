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
    public class SettingsViewModel : PageViewModel
    {
        // Initialization
        public SettingsViewModel()
        {
            ToDeckSelectionCommand = NewCommand(ToDeckSelection);
            AudioSettingsCommand = NewCommand(ToAudioSettings);
            CreditsCommand = NewCommand(ToCredits);
        }


        // Commands
        public IAsyncCommand ToDeckSelectionCommand { get; }
        public IAsyncCommand AudioSettingsCommand { get; }
        public IAsyncCommand CreditsCommand { get; }





        // Commands Handlers
        private async Task ToCredits()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToCredits();
            IsPageEnabled = true;
        }


        private async Task ToAudioSettings()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ShowAudioPopup();
            IsPageEnabled = true;
        }

        private async Task ToDeckSelection()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToDeckSelection();
            IsPageEnabled = true;
        }
    }
}
