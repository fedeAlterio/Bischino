using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using BischinoTheGame.ViewModel;

namespace BischinoTheGame.Model
{
    public class WaitingRoomInfo : ViewModelBase
    {
        public ObservableCollection<string> NotBotPlayers { get; set; }



        private int _botCounter;
        public int BotCounter
        {
            get => _botCounter;
            set => SetProperty(ref _botCounter, value);
        }
    }
}
