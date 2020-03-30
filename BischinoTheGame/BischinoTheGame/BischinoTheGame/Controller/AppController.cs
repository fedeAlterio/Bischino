using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller.Communication.ServerHandlers;
using Rooms.Controller.Navigation;
using Xamarin.Forms;

namespace Rooms.Controller
{
    public static class AppController
    {
        public static IRoomHandler RoomsHandler => RoomHandler.Instance;
        public static IAppNavigation Navigation { get; private set; }
        public static async void Start(INavigation navigation)
        {
            var appNavigation = new AppNavigation(navigation);
            Navigation = appNavigation;
            await appNavigation.Start();
        }
    }
}
