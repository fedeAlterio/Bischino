using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.Navigation;
using BischinoTheGame.View.Pages;
using BischinoTheGame.ViewModel.PageViewModels;
using Rg.Plugins.Popup.Services;
using Rooms.Controller;
using Xamarin.AdmobExample;
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
        }


        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            AppController.IsAppInBackground = true;
        }

        protected override void OnResume()
        {
            AppController.IsAppInBackground = false;
        }

        
    }
}
