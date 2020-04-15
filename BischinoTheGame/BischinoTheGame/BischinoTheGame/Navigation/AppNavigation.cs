using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame;
using BischinoTheGame.Navigation;
using BischinoTheGame.Navigation.RoomNavigation;
using BischinoTheGame.Navigation.TutorialNavigation;
using Rg.Plugins.Popup.Services;
using Rooms.Controller.Navigation;
using Xamarin.Forms;

namespace BischinoTheGame.Controller.Navigation
{
    public class AppNavigation : IAppNavigation
    {
        private readonly INavigation _navigation;


        private GameNavigation _roomNavigation;
        public IGameNavigation GameNavigation => _roomNavigation ??= new GameNavigation();


        private TutorialNavigation _tutorialNavigation;
        public ITutorialNavigation TutorialNavigation => _tutorialNavigation ??= new TutorialNavigation();



        public Task DisplayAlert(string title, string message)
        { 
            return Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public AppNavigation(INavigation navigation)
        {
            _navigation = navigation;
            PageNavigationBase.Navigation = navigation;
        }

        public Task Start() => GameNavigation.ToNameSelection(); // AppController.Settings.FirstRun ? TutorialNavigation.ToMainPage() :  RoomNavigation.ToNameSelection();

    }
}
