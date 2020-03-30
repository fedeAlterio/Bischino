using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class BetViewModel : ViewModelBase
    {
        private IList<int> _possibleBets;
        public IList<int> PossibleBets
        {
            get => _possibleBets;
            set => SetProperty(ref _possibleBets, value);
        }
    }
}
