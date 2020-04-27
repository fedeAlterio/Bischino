using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.ViewModel;

namespace BischinoTheGame.Model
{
    public class RoomManager : ViewModelBase
    {
        private bool _isGameStarted;
        public bool IsGameStarted
        {
            get => _isGameStarted;
            set => SetProperty(ref _isGameStarted, value);
        }



        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }



        private int _inGameTimeout;
        public int InGameTimeout
        {
            get => _inGameTimeout;
            set => SetProperty(ref _inGameTimeout, value);
        }
    }
}
