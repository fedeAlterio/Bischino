using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class BetInfoPopupViewModel : PageViewModel
    {
        public string InfoQuestion { get; } 

        public BetInfoPopupViewModel(int missingNumber)
        {
            InfoQuestion = $"Why can you not bet {missingNumber}?";
        }
    }
}
