﻿using System;
using Android.Gms.Ads;
using System.Threading.Tasks;
using BischinoTheGame.Droid.Implementations;
using Xamarin.AdmobExample;
using Xamarin.Forms;
using Android.Gms.Ads.Interstitial;

[assembly: Dependency(typeof(InterstitialAdsImplementation))]
namespace BischinoTheGame.Droid.Implementations
{
    public class InterstitialAdsImplementation : IInterstitialAds
    {
        public async Task Display(string adId)
        {
            // var displayTask = new TaskCompletionSource<bool>();
            //InterstitialAd adInterstitial = new InterstitialAd(context: Forms.Context)
            //{
            //    AdUnitId = adId
            //};
            //{
            //    var adInterstitialListener = new AdInterstitialListener(adInterstitial)
            //    {
            //        AdClosed = () =>
            //        {
            //            if (displayTask != null)
            //            {
            //                displayTask.TrySetResult(adInterstitial.IsLoaded);
            //                displayTask = null;
            //            }
            //        },
            //        AdFailed = () =>
            //        {
            //            if (displayTask != null)
            //            {
            //                displayTask.TrySetResult(adInterstitial.IsLoaded);
            //                displayTask = null;
            //            }
            //        }
            //    };

            //    AdRequest.Builder requestBuilder = new AdRequest.Builder();
            //    adInterstitial.AdListener = adInterstitialListener;
            //    adInterstitial.LoadAd(requestBuilder.Build());
            //}

            //return Task.WhenAll(displayTask.Task);
        }
    }

    public class AdInterstitialListener : AdListener
    {
       //private readonly InterstitialAd _interstitialAd;

       // public AdInterstitialListener(InterstitialAd interstitialAd)
       // {
       //     _interstitialAd = interstitialAd;
       // }

       // public Action AdLoaded { get; set; }
       // public Action AdClosed{ get; set; }
       // public Action AdFailed{ get; set; }

       // public override void OnAdLoaded()
       // {
       //     base.OnAdLoaded();

       //     if (_interstitialAd.IsLoaded)
       //     {
       //         _interstitialAd.Show();
       //     }
       //     AdLoaded?.Invoke();
       // }

       // public override void OnAdClosed()
       // {
       //     base.OnAdClosed();
       //     AdClosed?.Invoke();
       // }

       // public override void OnAdFailedToLoad(int errorCode)
       // {
       //     base.OnAdFailedToLoad(errorCode);
       //     AdFailed?.Invoke();
       // }
    }
}
