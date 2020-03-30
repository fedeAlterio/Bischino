using System;
using BischinoTheGame.Controller.Communication;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.View.Pages;
using BischinoTheGame.ViewModel.PageViewModels;
using Rg.Plugins.Popup.Services;
using Rooms.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BischinoTheGame
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage();
            AppController.Start(MainPage.Navigation);
            //Tmp();
        }

        private async void Tmp()
        {
            var query = new RoomQuery {PlayerName = "aa", RoomName = "df"};
            AppController.RoomsHandler.SubscribeMatchSnapshotUpdates(query);
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
