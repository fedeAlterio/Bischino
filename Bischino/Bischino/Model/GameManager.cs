using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model
{
    public class GameManager
    {
        public event EventHandler GameStartedEvent;
        public event EventHandler PhaseEndedEvent;
        public event EventHandler TurnEndedEvent; 
        public event EventHandler TurnStartedEvent; 
        public event EventHandler MatchUpdated;
        public event EventHandler<Player> CurrentPlayerChanged;
        public event EventHandler<IList<Player>> EndOfMatch;


        private const int EndPhaseDelay = 3 * 1000;
        public string RoomName { get; }
        public Deck Deck { get; private set; }
        public bool TurnEnded { get; private set; }
        public bool PhaseEnded { get; private set; }
        public bool GameEnded { get; private set; }
        public Player PhaseStarter { get; private set; }
        public Player TurnStarter { get; private set; }
        public IList<Player> Winners { get; private set; }
        public int Version { get; private set; } = 0;



        private Player _currentPlayer;
        public Player CurrentPlayer
        {
            get => _currentPlayer;
            private set
            {
                _currentPlayer = value;
                CurrentPlayerChanged?.Invoke(this, _currentPlayer);
            }
        }



        private List<Player> _players;
        public IReadOnlyList<Player> Players => _players;



        private List<Player> _playingPlayers;
        public IReadOnlyList<Player> PlayingPlayers => _playingPlayers;



        private List<Card> _droppedCards;
        public IReadOnlyList<Card> DroppedCards => _droppedCards;



        private List<int> _bets;
        public IReadOnlyList<int> Bets => _bets;


        private IList<Player> _toRemove;



        public GameManager(string roomName, IEnumerable<string> playerNames, int botNumber = 0)
        {
            RoomName = roomName;
            _players = new List<Player>();
            foreach (var name in playerNames)
                NewPlayer(name);

            for(var i=0; i < botNumber; i++)
                NewPlayer($"Bot{i}", true);
        }



        private void NewPlayer(string name, bool isBot = false)
        {
            var player = isBot ? new Bot(this, name) : new Player(this, name);
            player.NewBetEvent += Player_NewBetEvent;
            player.DroppedCardEvent += Player_DroppedCardEvent;
            player.HasLostEvent += Player_HasLostEvent;
            player.PlayerWinEvent += Player_PlayerWinEvent;
            _players.Add(player);
        }



        private void Player_PlayerWinEvent(object sender, EventArgs e)
        {
            PhaseStarter = sender as Player;
            CurrentPlayer = PhaseStarter;
        }



        private void Player_HasLostEvent(object sender, EventArgs e)
        {
            _toRemove.Add(sender as Player);
        }



        public void StartGame()
        {
            _playingPlayers = new List<Player>(Players);
            Winners = null;
            TurnStarter = _playingPlayers[0];
            PhaseStarter = TurnStarter;
            CurrentPlayer = PhaseStarter;
            _droppedCards = new List<Card>();
            _toRemove = new List<Player>();
            _bets = new List<int>();
            Deck = new Deck();
            TurnEnded = false;
            GameStartedEvent?.Invoke(this, EventArgs.Empty);
            CurrentPlayer.StartBetPhase();

            MatchUpdated?.Invoke(this, EventArgs.Empty);
        }



        private void Player_NewBetEvent(object sender, int bet)
        {
            _bets.Add(bet);
            NextPlayer();
            if(CurrentPlayer == PhaseStarter)
                CurrentPlayer.StartDropPhase();
            else 
                CurrentPlayer.StartBetPhase();
            Version++;

            MatchUpdated?.Invoke(this, EventArgs.Empty);
        }



        private void Player_DroppedCardEvent(object sender, Card e)
        {
            _droppedCards.Add(e);
            NextPlayer();
            if(CurrentPlayer == PhaseStarter)
                if (CurrentPlayer.Cards.Count == 0)
                   EndTurn();
                else
                   EndPhase();
            else 
                CurrentPlayer.StartDropPhase();
            Version++;

            MatchUpdated?.Invoke(this, EventArgs.Empty);
        }




        private async void EndPhase()
        {
            PhaseEndedEvent?.Invoke(this, EventArgs.Empty);
            PhaseEnded = true;
            Version++;

            await Task.Delay(EndPhaseDelay);
            NewPhase();
        }


        public void NotifyIdled(string playerName)
        {
            var player = _playingPlayers.Find(p => p.Name == playerName);

            player.NotifyIdled();
            RemovePlayers();
            SetupNextTurn();
            NewTurn();
            Version++;
        }


        private void RemovePlayers()
        {
            foreach (var player in _toRemove)
                _playingPlayers.Remove(player);
            _toRemove = new List<Player>();
        }



        private void SetupNextTurn()
        {
            var totLost = from p in Players where p.HasLost select p;
            GameEnded = totLost.Count() >= Players.Count - 1;
            if (GameEnded)
                ToGameEnd();
            else
            {
                TurnStarter = FindNextPlaying(TurnStarter);
                PhaseStarter = TurnStarter;
                CurrentPlayer = PhaseStarter;
            }
        }



        public async void EndTurn()
        {
            TurnEnded = true;
            TurnEndedEvent?.Invoke(this, EventArgs.Empty);

            RemovePlayers();
            SetupNextTurn();
            Version++;

            await Task.Delay(EndPhaseDelay);
            NewTurn();
        }



        private void ToGameEnd()
        {
            var minLost = _players.Min(p => p.TotLost);
            var winnersEnum = from p in _players where p.TotLost == minLost select p;
            var winners = winnersEnum.ToList();
            Winners = winners;
            EndOfMatch?.Invoke(this, winners);
            Version++;
        }



        public void NewPhase()
        {
            _droppedCards = new List<Card>();
            PhaseEnded = false;
            CurrentPlayer.StartDropPhase();
            Version++;

            MatchUpdated?.Invoke(this, EventArgs.Empty);
        }


        public void NewTurn()
        {
            if (GameEnded)
                return;

            TurnEnded = false;
            Deck.Shuffle();
            _droppedCards = new List<Card>();
            _bets = new List<int>();
            TurnStartedEvent?.Invoke(this, EventArgs.Empty);
            CurrentPlayer.StartBetPhase();
            Version++;

            MatchUpdated?.Invoke(this, EventArgs.Empty);
        }



        public IList<Card> GetOtherPlayersCards(Player player)
        {
            var query = from p in _playingPlayers where p != player from c in p.Cards select c;
            var cards = query.ToList();
            return cards;
        }



        public MatchSnapshot GetSnapshot(string playerName)
        {
            var player = Players.FirstOrDefault(p => p.Name == playerName);
            if (player is null)
                throw new Exception("Player does not exist");

            var otherPlayers = (from p in Players select PrivatePlayer.FromPlayer(p)).ToList();

            IList<PrivatePlayer> winners = null;
            if (Winners is {})
                winners = (from p in Winners select PrivatePlayer.FromPlayer(p)).ToList();

            var ret = new MatchSnapshot
            {
                Player = player,
                OtherPlayers = otherPlayers,
                DroppedCards = _droppedCards,
                Host = Players[0].Name,
                IsEndTurn = TurnEnded,
                IsMatchEnded = GameEnded,
                IsPhaseEnded = PhaseEnded,
                PlayerTurn = PrivatePlayer.FromPlayer(CurrentPlayer),
                Winners = winners,
                Version = Version
            };
            return ret;
        }



        private void NextPlayer()
        {
            CurrentPlayer = CurrentPlayer == _playingPlayers.Last()
                ? _playingPlayers[0]
                : _playingPlayers[_playingPlayers.IndexOf(CurrentPlayer) + 1];
        }




        private Player FindNextPlaying(Player p)
        {
            if (!Players.Contains(p))
                throw new Exception("Player not found");

            var i = _players.Last() == p ? 0 : _players.IndexOf(p) + 1;
            for (; !_playingPlayers.Contains(_players[i]); i = _players[i] == _players.Last() ? 0 : i + 1)
                ;
            return _players[i];
        }
    }
}
