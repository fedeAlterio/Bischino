using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bischino.Base.Model;

namespace Bischino.Bischino
{
    public class Player : ModelBase
    {
        public event EventHandler<Card> DroppedCardEvent;
        public event EventHandler<int> NewBetEvent;
        public event EventHandler HasLostEvent;
        public event EventHandler PlayerWinEvent;  

        private readonly GameManager _gameManager;
        private IList<int> _possibleBets;

        public string Name { get; }
        public int? WinBet { get; private set; }
        public int? TotLost { get; private set; }
        public int? PhaseWin{ get; private set; }
        public int StartCardsCount { get; private set; }
        public Card Dropped { get; private set; }
        public DropCardViewModel DropCardViewModel { get; private set; }
        public BetViewModel BetViewModel { get; private set; }
        public LastPhaseViewModel LastPhaseViewModel { get; set; }


        private List<Card> _cards;
        public IReadOnlyList<Card> Cards => _cards;


        public Player(GameManager gameManager, string name)
        {
            _gameManager = gameManager;
            Name = name;
            gameManager.GameStartedEvent += StartGame;
            gameManager.TurnEndedEvent += EndTurn;
            gameManager.PhaseEndedEvent += EndPhase;
            gameManager.TurnStartedEvent += NewTurn;
        }

        private void NewTurn(object sender, EventArgs e)
        {
            WinBet = null;
            PhaseWin = 0;
            Dropped = null;
            StartCardsCount = StartCardsCount == 1 ? 5 : StartCardsCount - 1;
            _cards = new List<Card>(_gameManager.Deck.Draw(StartCardsCount, Name));
        }

        private void EndPhase(object sender, EventArgs e)
        {
            var hasWin = _gameManager.DroppedCards.All(card => card.Value <= Dropped.Value);
            if (hasWin)
            {
                PhaseWin++;
                PlayerWinEvent?.Invoke(this, EventArgs.Empty);
            }
            Dropped = null;
        }

        private void EndTurn(object sender, EventArgs e)
        {
            EndPhase(sender, e);
            CalculatePoints();
            if (HasLost)
            {
                WinBet = null;
                _cards.Clear();
                PhaseWin = 0;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            WinBet = null;
            TotLost = 0;
            PhaseWin = 0;
            StartCardsCount = 5;
            Dropped = null;
            DropCardViewModel = null;
            BetViewModel = null;
            _cards = new List<Card>(_gameManager.Deck.Draw(StartCardsCount, Name));
            _possibleBets = null;
        }


        public void StartBetPhase()
        {
            _possibleBets = CalculatePossibleBets();
            if (Cards.Count == 1)
            {
                var cards = _gameManager.GetOtherPlayersCards(this);
                LastPhaseViewModel = new LastPhaseViewModel {Cards = cards, CanBetLose = _possibleBets.Contains(0), CanBetWin = _possibleBets.Contains(1)};
            }
            else
                BetViewModel = new BetViewModel {PossibleBets = _possibleBets};
        }

        private IList<int> CalculatePossibleBets()
        {
            var currentBets = _gameManager.Bets;
            var sum = currentBets.Sum();
            var isLastPlayer = currentBets.Count == _gameManager.PlayingPlayers.Count - 1;
            var ret = new List<int>();
            for(var i=0; i < Cards.Count + 1; i++)
                if(!isLastPlayer || i + sum != Cards.Count)
                    ret.Add(i);
            return ret;
        }


        public void NewBet(int bet)
        {
            if (_possibleBets is null || !_possibleBets.Contains(bet))
                throw new Exception("Impossible bet");
            if (_gameManager.CurrentPlayer != this)
                throw new Exception("Wrong turn");

            WinBet = bet;
            BetViewModel = null;
            LastPhaseViewModel = null;
            _possibleBets = null;
            NewBetEvent?.Invoke(this, bet);
        }



        public void StartDropPhase()
        {
            DropCardViewModel = new DropCardViewModel {Cards = _cards};
        }


        public void DropCard(string cardName)
        {
            var card = Cards.FirstOrDefault(c => c.Name == cardName);
            if(card is null)
                throw new Exception("Impossible drop a card that does not exist");
            if (_gameManager.CurrentPlayer != this)
                throw new Exception("Wrong turn");

            DropCardViewModel = null;
            _cards.Remove(card);
            Dropped = card;
            DroppedCardEvent?.Invoke(this, card);
        }


        public void DropPaolo(bool isMax)
        {
            var paolo = Cards.FirstOrDefault(c => c.IsPaolo);
            if(paolo is null)
                throw new Exception("Paolo not found");

            paolo.Value = isMax ? 40 : -1;
            DropCard(paolo.Name);
        }


        private void CalculatePoints()
        {
            if(WinBet is null)
                throw new Exception($"Null bet for player {Name} ");
            if (PhaseWin == null)
                throw new Exception($"Phase bet null for player {Name}");

            TotLost += Math.Abs(WinBet.Value - PhaseWin.Value);
            if(HasLost)
            {
                UnsubscribeFromEvents();
                HasLostEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        private void UnsubscribeFromEvents()
        {
            _gameManager.GameStartedEvent -= StartGame;
            _gameManager.TurnEndedEvent -= EndTurn;
            _gameManager.PhaseEndedEvent -= EndPhase;
            _gameManager.TurnStartedEvent -= NewTurn;
        }


        public bool HasLost => TotLost >= 3;

    }
}
