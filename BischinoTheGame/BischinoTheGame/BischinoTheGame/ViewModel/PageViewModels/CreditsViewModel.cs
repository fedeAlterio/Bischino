using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class CreditsViewModel : PageViewModel
    {
        private Command _musicCreditsCommand;
        public Command MusicCreditsCommand
        {
            get => _musicCreditsCommand;
            set => SetProperty(ref _musicCreditsCommand, value);
        }


        private Command _playAnimationCommand;
        public Command PlayAnimationCommand
        {
            get => _playAnimationCommand;
            set => SetProperty(ref _playAnimationCommand, value);
        }


        private Command _bouncingCoinAnimationCommand;
        public Command BouncingCoinAnimationCommand
        {
            get => _bouncingCoinAnimationCommand;
            set => SetProperty(ref _bouncingCoinAnimationCommand, value);
        }


        private Command _fireWorksAnimationCommand;
        public Command FireWorksAnimationCommand
        {
            get => _fireWorksAnimationCommand;
            set => SetProperty(ref _fireWorksAnimationCommand, value);
        }


        public CreditsViewModel()
        {
            MusicCreditsCommand = new Command(_ => OpenUri(new Uri("http://freesfx.co.uk")));
            PlayAnimationCommand = new Command(_ => OpenUri(new Uri("https://lottiefiles.com/user/246578")));
            BouncingCoinAnimationCommand = new Command(_ => OpenUri(new Uri("https://lottiefiles.com/the__creador")));
            FireWorksAnimationCommand = new Command(_ => OpenUri(new Uri("https://lottiefiles.com/user/61955")));
        }



        private async void OpenUri(Uri uri)
        {
            IsPageEnabled = false;
            await Launcher.OpenAsync(uri);
            IsPageEnabled = true;
        }
    }
}
