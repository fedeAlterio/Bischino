using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class BetInfoPopupViewModel : PageViewModel
    {
        private string _infoQuestion;
        public string InfoQuestion
        {
            get => _infoQuestion;
            set => SetProperty(ref _infoQuestion, value);
        }


        public BetInfoPopupViewModel(int missingNumber)
        {
            InfoQuestion = $"Why can you not bet {missingNumber}?";
        }
    }
}
