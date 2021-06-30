using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class AudioPopupViewModel : PageViewModel
    {
        public Command SoundEffectsOnCommand { get; }
        public Command SoundEffectsOffCommand { get; }
        public Command AudioOnCommand { get; }
        public Command AudioOffCommand { get; }
        public Command NextTrackCommand { get; }
        public Command PreviousTrackCommand { get; }

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
