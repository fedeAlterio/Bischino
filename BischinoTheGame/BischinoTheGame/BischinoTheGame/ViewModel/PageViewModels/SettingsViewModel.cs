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
            await AppController.Navigation.GameNavigation.ToCredits();
        }


        private async Task ToAudioSettings()
        {
            await AppController.Navigation.GameNavigation.ShowAudioPopup();
        }

        private async Task ToDeckSelection()
        {
            await AppController.Navigation.GameNavigation.ToDeckSelection();
        }
    }
}
