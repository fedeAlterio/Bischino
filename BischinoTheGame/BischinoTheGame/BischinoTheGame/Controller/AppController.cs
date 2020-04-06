using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.ServerHandlers;
using BischinoTheGame.Model.Settings;
using Rooms.Controller.Navigation;
using Xamarin.Forms;

namespace Rooms.Controller
{
    public static class AppController
    {
        public static IRoomHandler RoomsHandler => RoomHandler.Instance;
        public static IAppNavigation Navigation { get; private set; }
        public static AudioManager AudioManager { get; private set; }
        public static SettingsManager Settings { get; private set; }
        public static async void Start(INavigation navigation)
        {
            var appNavigation = new AppNavigation(navigation);
            Navigation = appNavigation;
            Settings = new SettingsManager();
            AudioManager = new AudioManager {AudioOn = Settings.AudioOn};
            await appNavigation.Start();
        }
    }
}
