using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.ServerHandlers;
using BischinoTheGame.Controller.Navigation;
using BischinoTheGame.Model.Settings;
using BischinoTheGame.Navigation;
using Rooms.Controller.Navigation;
using Xamarin.Forms;

namespace BischinoTheGame.Controller
{
    public static class AppController
    {
        public static IGameHandler GameHandler => Communication.ServerHandlers.GameHandler.Instance;
        public static IAppNavigation Navigation { get; private set; }
        public static AudioManager AudioManager { get; private set; }
        public static SettingsManager Settings { get; private set; }
        public static AdHandler AdHandler { get; private set; }

        private static bool _isAppInBackground;
        public static bool IsAppInBackground
        {
            get => _isAppInBackground;
            set
            {
                _isAppInBackground = value;
                if(value)
                    AudioManager.GoingBackground();
                else 
                    AudioManager.OnResume();
            }
        }

        public static async void Start(INavigation navigation)
        {
            var appNavigation = new AppNavigation(navigation);
            Navigation = appNavigation;
            Settings = new SettingsManager();
            AudioManager = new AudioManager {AudioOn = Settings.AudioOn};
            AdHandler = new AdHandler();

            await appNavigation.Start();
        }
    }
}
