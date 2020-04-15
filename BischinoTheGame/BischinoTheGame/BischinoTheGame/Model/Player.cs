using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.ViewModel;
using BischinoTheGame.ViewModel.PageViewModels;

namespace BischinoTheGame.Model
{
    public class Player : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }



        private int _startCardsCount;
        public int StartCardsCount
        {
            get => _startCardsCount;
            set => SetProperty(ref _startCardsCount, value);
        }



        private int? _winBet;
        public int? WinBet
        {
            get => _winBet;
            set => SetProperty(ref _winBet, value);
        }



        private int? _totLost;
        public int? TotLost
        {
            get => _totLost;
            set => SetProperty(ref _totLost, 3 -value);
        }



        private int? _phaseWin;
        public int? PhaseWin
        {
            get => _phaseWin;
            set => SetProperty(ref _phaseWin, value);
        }



        private IList<Card> _cards;
        public IList<Card> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }



        private IList<Card> _droppedCards;
        public IList<Card> DroppedCards
        {
            get => _droppedCards;
            set => SetProperty(ref _droppedCards, value);
        }



        private IList<bool> _winHistory;
        public IList<bool> WinHistory
        {
            get => _winHistory;
            set => SetProperty(ref _winHistory, value);
        }



        private DropCardViewModel _dropCardViewModel;
        public DropCardViewModel DropCardViewModel
        {
            get => _dropCardViewModel;
            set => SetProperty(ref _dropCardViewModel, value);
        }



        private BetViewModel _propertyName;
        public BetViewModel BetViewModel
        {
            get => _propertyName;
            set => SetProperty(ref _propertyName, value);
        }



        private LastPhaseViewModel _lastPhaseViewModel;
        public LastPhaseViewModel LastPhaseViewModel
        {
            get => _lastPhaseViewModel;
            set => SetProperty(ref _lastPhaseViewModel, value);
        }


        public bool HasLost => _totLost <= 0;
    }
}
