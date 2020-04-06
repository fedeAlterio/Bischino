using System;
using System.Collections.Generic;
using System.Text;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class AudioPopupViewModel : PageViewModel
    {


        private Command _audioOnCommand;
        public Command AudioOnCommand
        {
            get => _audioOnCommand;
            set => SetProperty(ref _audioOnCommand, value);
        }


        private Command _audioOffCommand;
        public Command AudioOffCommand
        {
            get => _audioOffCommand;
            set => SetProperty(ref _audioOffCommand, value);
        }


        private Command _nextTrackCommand;
        public Command NextTrackCommand
        {
            get => _nextTrackCommand;
            set => SetProperty(ref _nextTrackCommand, value);
        }


        private Command _previousTrackCommand;
        public Command PreviousTrackCommand
        {
            get => _previousTrackCommand;
            set => SetProperty(ref _previousTrackCommand, value);
        }


        private Command _soundEffectsOnCommand;
        public Command SoundEffectsOnCommand
        {
            get => _soundEffectsOnCommand;
            set => SetProperty(ref _soundEffectsOnCommand, value);
        }


        private Command _soundEffectsOffCommand;
        public Command SoundEffectsOffCommand
        {
            get => _soundEffectsOffCommand;
            set => SetProperty(ref _soundEffectsOffCommand, value);
        }


        public AudioPopupViewModel()
        {
            AudioOnCommand = new Command(_ => AppController.AudioManager.AudioOn = true);
            AudioOffCommand = new Command(_ => AppController.AudioManager.AudioOn = false);
            NextTrackCommand = new Command(_ => AppController.AudioManager.PlayNext());
            PreviousTrackCommand = new Command(_ => AppController.AudioManager.PlayPrevious());
            SoundEffectsOnCommand = new Command(_ => AppController.Settings.SoundEffectsOn = true);
            SoundEffectsOffCommand = new Command(_ => AppController.Settings.SoundEffectsOn = false);
        }
    }
}
