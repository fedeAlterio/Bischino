using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class AudioPopupViewModel : PageViewModel
    {
        public IAsyncCommand SoundEffectsOnCommand { get; }
        public IAsyncCommand SoundEffectsOffCommand { get; }
        public IAsyncCommand AudioOnCommand { get; }
        public IAsyncCommand AudioOffCommand { get; }
        public IAsyncCommand NextTrackCommand { get; }
        public IAsyncCommand PreviousTrackCommand { get; }

        public AudioPopupViewModel()
        {
            AudioOnCommand = NewCommand(() => AppController.AudioManager.AudioOn = true);
            AudioOffCommand = NewCommand(() => AppController.AudioManager.AudioOn = false);
            NextTrackCommand = NewCommand(() => AppController.AudioManager.PlayNext());
            PreviousTrackCommand = NewCommand(() => AppController.AudioManager.PlayPrevious());
            SoundEffectsOnCommand = NewCommand(() => AppController.Settings.SoundEffectsOn = true);
            SoundEffectsOffCommand = NewCommand(() => AppController.Settings.SoundEffectsOn = false);
        }
    }
}
