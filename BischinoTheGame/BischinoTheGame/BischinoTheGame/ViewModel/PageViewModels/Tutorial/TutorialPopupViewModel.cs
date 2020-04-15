using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels.Tutorial
{
    public class TutorialPopupViewModel : PageViewModel
    {
        private Command _tutorialCommand;
        public Command TutorialCommand
        {
            get => _tutorialCommand;
            set => SetProperty(ref _tutorialCommand, value);
        }


        public TutorialPopupViewModel()
        {
            TutorialCommand = new Command(_ => LaunchTutorial());
        }

        private async void LaunchTutorial()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.LaunchTutorial();
            IsPageEnabled = true;
        }
    }
}
