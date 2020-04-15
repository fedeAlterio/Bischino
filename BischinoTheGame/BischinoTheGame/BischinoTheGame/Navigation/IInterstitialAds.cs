using System;
using System.Threading.Tasks;

namespace Xamarin.AdmobExample
{
    public interface IInterstitialAds
    {
         Task Display(string adId); 
    }
}
