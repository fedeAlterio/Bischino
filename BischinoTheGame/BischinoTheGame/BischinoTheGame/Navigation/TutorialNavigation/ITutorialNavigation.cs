using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BischinoTheGame.Navigation.TutorialNavigation
{
    public interface ITutorialNavigation
    {
        Task ToMainPage();
        Task NotifyTutorialEnded();
    }
}
