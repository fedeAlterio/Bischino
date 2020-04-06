using System;
using System.IO;
using System.Reflection;
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
            await AppController.Navigation.RoomNavigation.ShowAudioPopup();
        }


        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            AppController.AudioManager.GoingBackground();
        }

        protected override void OnResume()
        {
            AppController.AudioManager.OnResume();
        }
    }
}
