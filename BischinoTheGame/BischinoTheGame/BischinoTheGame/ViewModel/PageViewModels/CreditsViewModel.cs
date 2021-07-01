using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class CreditsViewModel : PageViewModel
    {

        // Initilization
        public CreditsViewModel()
        {
            MusicCreditsCommand = NewCommand(async () => await OpenUri(new Uri("http://freesfx.co.uk")));
            PlayAnimationCommand = NewCommand(async () => await OpenUri(new Uri("https://lottiefiles.com/user/246578")));
            BouncingCoinAnimationCommand = NewCommand(async () => await OpenUri (new Uri("https://lottiefiles.com/the__creador")));
            FireWorksAnimationCommand = NewCommand(async () => await OpenUri(new Uri("https://lottiefiles.com/user/61955")));
        }

        // Comands
        public IAsyncCommand MusicCreditsCommand { get; }
        public IAsyncCommand PlayAnimationCommand { get; }
        public IAsyncCommand BouncingCoinAnimationCommand { get; }
        public IAsyncCommand FireWorksAnimationCommand { get; }


        // Commands Handlers
        private async Task OpenUri(Uri uri)
        {
            await Launcher.OpenAsync(uri);
        }
    }
}
