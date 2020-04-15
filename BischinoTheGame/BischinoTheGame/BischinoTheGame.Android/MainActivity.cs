    using System;

using Android.App;
using Android.Content.PM;
    using Android.Gms.Ads;
    using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BischinoTheGame.View;
using BischinoTheGame.View.Pages;
    using BischinoTheGame.View.Pages.Tutorial;
    using Lottie.Forms.Droid;
using Xamarin.Forms;
    using Application = Android.App.Application;

    namespace BischinoTheGame.Droid
{
    [Activity(Label = "Bischino", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.Views.View.IOnSystemUiVisibilityChangeListener
    {
        int uiOptions;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
           
            MobileAds.Initialize(Application.Context, "ca-app-pub-7000661273633463~4636561772");

            LoadApplication(new App());
            Initialize();
            AnimationViewRenderer.Init();
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            uiOptions = (int)Window.DecorView.SystemUiVisibility;

            uiOptions |= (int)SystemUiFlags.LowProfile;
            uiOptions |= (int)SystemUiFlags.Fullscreen;
            uiOptions |= (int)SystemUiFlags.HideNavigation;
            uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            Window.DecorView.SetOnSystemUiVisibilityChangeListener(this);
        }

        private void Initialize()
        {
            OrientationSetup();
           
        }

        private void OrientationSetup<T>() where T : class
        {
            MessagingCenter.Subscribe<T>(this, ViewMessagingConstants.Unspecified,
                sender => RequestedOrientation = ScreenOrientation.Unspecified);

            MessagingCenter.Subscribe<T>(this, ViewMessagingConstants.Landscape,
                sender => RequestedOrientation = ScreenOrientation.Landscape);
        }

        private void OrientationSetup()
        {
            OrientationSetup<GamePage>();
            OrientationSetup<DeckSelectionPage>();
            OrientationSetup<TutorialMainPage>();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnSystemUiVisibilityChange([GeneratedEnum] StatusBarVisibility visibility)
        {
            if (((int)visibility & (int)SystemUiFlags.Fullscreen) == 0)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
            }

        }
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)((int)SystemUiFlags.Fullscreen
                                                                        | (int)SystemUiFlags.ImmersiveSticky
                                                                        | (int)SystemUiFlags.LayoutHideNavigation
                                                                        | (int)SystemUiFlags.LayoutStable
                                                                        | (int)SystemUiFlags.HideNavigation);
        }
    }
}