using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using BischinoTheGame.Model;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class MatchSnapshot : ViewModelBase
    {
        private Player _player;
        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }



        private ObservableCollection<Card> _droppedCards;
        public ObservableCollection<Card> DroppedCards
        {
            get => _droppedCards;
            set => SetProperty(ref _droppedCards, value);
        }



        private ObservableCollection<PrivatePlayer> _otherPlayers;
        public ObservableCollection<PrivatePlayer> OtherPlayers
        {
            get => _otherPlayers;
            set => SetProperty(ref _otherPlayers, value);
        }




        private bool _isMatchEnded;
        public bool IsMatchEnded
        {
            get => _isMatchEnded;
            set => SetProperty(ref _isMatchEnded, value);
        }



        private bool _isEndTurn;
        public bool IsEndTurn
        {
            get => _isEndTurn;
            set => SetProperty(ref _isEndTurn, value);
        }



        private bool _isPhaseEnded;
        public bool IsPhaseEnded
        {
            get => _isPhaseEnded;
            set => SetProperty(ref _isPhaseEnded, value);
        }



        private PrivatePlayer _playerTurn;
        public PrivatePlayer PlayerTurn
        {
            get => _playerTurn;
            set => SetProperty(ref _playerTurn, value);
        }



        private IList<PrivatePlayer> _winners;
        public IList<PrivatePlayer> Winners
        {
            get => _winners;
            set => SetProperty(ref _winners, value);
        }



        private int _version;
        public int Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }
    }
}