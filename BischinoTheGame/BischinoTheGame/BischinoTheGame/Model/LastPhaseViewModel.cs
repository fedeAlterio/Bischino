using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using BischinoTheGame.ViewModel;

namespace BischinoTheGame.Model
{
    public class LastPhaseViewModel : ViewModelBase
    {
        public ObservableCollection<Card> Cards { get; set; }


        private bool _canBetWin;
        public bool CanBetWin
        {
            get => _canBetWin;
            set => SetProperty(ref _canBetWin, value);
        }


        private bool _canBetLose;
        public bool CanBetLose
        {
            get => _canBetLose;
            set => SetProperty(ref _canBetLose, value);
        }
    }
}
