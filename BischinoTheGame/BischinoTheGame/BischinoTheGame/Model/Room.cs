using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.ViewModel;

namespace BischinoTheGame.Model
{
    public class Room : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        private string _host;
        public string Host
        {
            get => _host;
            set => SetProperty(ref _host, value);
        }


        private int? _minPlayers;
        public int? MinPlayers
        {
            get => _minPlayers;
            set => SetProperty(ref _minPlayers, value);
        }


        private int? _maxPlayers;
        public int? MaxPlayers
        {
            get => _maxPlayers;
            set => SetProperty(ref _maxPlayers, value);
        }


        private IList<string> _pendingPlayers;
        public IList<string> PendingPlayers
        {
            get => _pendingPlayers;
            set => SetProperty(ref _pendingPlayers, value);
        }
    }
}
