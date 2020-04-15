using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.View.Pages.Tutorial;
using BischinoTheGame.ViewModel.PageViewModels.Tutorial;
using Rooms.Controller;
using Rooms.Controller.Navigation;

namespace BischinoTheGame.Navigation.TutorialNavigation
{
    public class TutorialNavigation : PageNavigationBase, ITutorialNavigation
    {

        public async Task ToMainPage()
        {
            var vm = new TutorialMainViewModel();
            var page = new TutorialMainPage {BindingContext = vm};
            await Navigation.PushAsync(page);
        }

        public Task NotifyTutorialEnded()
        {
            AppController.Settings.FirstRun = false;
            return Navigation.NavigationStack.Count > 1 ? Navigation.PopAsync() : AppController.Navigation.GameNavigation.ToNameSelection();
        }
    }
    
}
