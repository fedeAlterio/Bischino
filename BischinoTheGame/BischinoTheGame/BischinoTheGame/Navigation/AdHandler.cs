using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.AdmobExample;
using Xamarin.Forms;

namespace BischinoTheGame.Navigation
{
    public class AdHandler
    {
        public const string InterstitialAdId1 = "ca-app-pub-7000661273633463/6084594562";
        public Task ShowInterstitial1() => DependencyService.Get<IInterstitialAds>().Display(InterstitialAdId1);
    }
}
