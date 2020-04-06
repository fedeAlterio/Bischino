using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame;
using BischinoTheGame.Navigation.RoomNavigation;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace Rooms.Controller.Navigation
{
    public class AppNavigation : IAppNavigation
    {
        private readonly INavigation _navigation;

        private RoomNavigation _roomNavigation;
        public IRoomNavigation RoomNavigation => _roomNavigation ??= new RoomNavigation();
        public async Task PopPopupAsync()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public Task DisplayAlert(string title, string message)
        { 
            return Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public AppNavigation(INavigation navigation)
        {
            _navigation = navigation;
            PageNavigationBase.Navigation = navigation;
        }

        public async Task Start()
        {
            await RoomNavigation.ToNameSelection();
        }
    }
}
