using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Widget;
using Android.Gms.Ads;
using BischinoTheGame.Controls;
using BischinoTheGame.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdControlView), typeof(AdViewRenderer))]
namespace BischinoTheGame.Droid.Renderer
{
    public class AdViewRenderer : ViewRenderer<Controls.AdControlView, AdView>
    {
        private readonly AdSize _adSize = AdSize.SmartBanner;
        private AdView _adView;

        public AdViewRenderer(Context context) : base(context) { }

        private AdView CreateNativeAdControl(AdControlView adView)
        {
            if (_adView != null)
                return _adView;
            
            _adView = new AdView(Context)
            {
                AdSize = _adSize,
                AdUnitId = adView.AdUnitId,
                LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent)
            };
            
            _adView.LoadAd(new AdRequest.Builder().Build());
            return _adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Controls.AdControlView> e)
        {
            base.OnElementChanged(e);
            if (Control != null)
                return;

            CreateNativeAdControl(e.NewElement);
            SetNativeControl(_adView);
        }
    }
}