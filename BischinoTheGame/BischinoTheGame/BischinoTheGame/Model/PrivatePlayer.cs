using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.ViewModel;

namespace BischinoTheGame.Model
{
    public class PrivatePlayer : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        private int _cardCount;
        public int CardCount
        {
            get => _cardCount;
            set => SetProperty(ref _cardCount, value);
        }


        private int? _winBet;
        public int? WinBet
        {
            get => _winBet;
            set => SetProperty(ref _winBet, value);
        }


        private int? _phaseWin;
        public int? PhaseWin
        {
            get => _phaseWin;
            set => SetProperty(ref _phaseWin, value);
        }

        private int? _totLost;
        public int? TotLost
        {
            get => _totLost;
            set => SetProperty(ref _totLost, 3 - value);
        }


        private bool _hasLost;
        public bool HasLost
        {
            get => _hasLost;
            set => SetProperty(ref _hasLost, value);
        }
    }
}
