using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Model;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class DropCardViewModel : PageViewModel
    {
        private IList<Card> _cards;
        public IList<Card> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }
    }
}
