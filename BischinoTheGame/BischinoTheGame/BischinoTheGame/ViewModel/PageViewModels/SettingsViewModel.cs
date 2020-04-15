using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
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


        private Command _audioSettingsCommand;
        public Command AudioSettingsCommand
        {
            get => _audioSettingsCommand;
            set => SetProperty(ref _audioSettingsCommand, value);
        }


        private Command _creditsCommand;
        public Command CreditsCommand
        {
            get => _creditsCommand;
            set => SetProperty(ref _creditsCommand, value);
        }



        public SettingsViewModel()
        {
            ToDeckSelectionCommand = new Command(_ => ToDeckSelection());
            AudioSettingsCommand = new Command(_ => ToAudioSettings());
            CreditsCommand = new Command(_ => ToCredits());
        }

        private async void ToCredits()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToCredits();
            IsPageEnabled = true;
        }


        private async void ToAudioSettings()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ShowAudioPopup();
            IsPageEnabled = true;
        }

        private async void ToDeckSelection()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToDeckSelection();
            IsPageEnabled = true;
        }
    }
}
