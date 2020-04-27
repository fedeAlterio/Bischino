using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bischino.Model
{
    public class Bot : Player
    {
        private const int ThinkTime = 2 * 1000;
        private List<Card> _winCards;
        public Bot(GameManager gameManager, string name) : base(gameManager, name)
        {
        }



        public override async void StartBetPhase()
        {
            base.StartBetPhase();

            var bet = Cards.Count == 1 ? MakeOneCardBet() : MakeABet();

            await Task.Delay(ThinkTime);
            NewBet(bet);
        }



        public override async void StartDropPhase()
        {
            base.StartDropPhase();

            var card = ChooseToDrop();

            if (_winCards != null && _winCards.Contains(card))
                _winCards.Remove(card);

            if (card.IsPaolo && card.Value == 30)
                card.Value = -1;
            await Task.Delay(ThinkTime);
            DropCard(card.Name);
        }



        private int GetNear(int bet, IEnumerable<int> possibleBets, bool hasPaolo)
        {
            var possibleChoices = possibleBets as int[] ?? possibleBets.ToArray();
            if (possibleChoices.Contains(bet))
                return bet;

            if(hasPaolo)
                return possibleChoices.Contains(bet + 1) ?  bet + 1 : bet - 1;

            var goodCards = from card in Cards
                where !card.IsPaolo && !_winCards.Contains(card) && card.Value >= 20
                select card;

            if(goodCards.Any())
                return possibleChoices.Contains(bet + 1) ? bet + 1 : bet - 1;

            return possibleChoices.Contains(bet - 1) ? bet - 1 : bet + 1;
        }



        private int GetThreshold(int totCards, int totPlayers)
        {
            if (totPlayers <= 3)
                return totCards switch
                {
                    5 => 3,
                    4 => 2,
                    3 => 2,
                    2 => 2,
                    1 => 1,
                    _ => 0
                };

            if (totPlayers == 4)
               return totCards switch 
                {
                    5 => 2,
                    4 => 1,
                    3 => 1,
                    2 => 0,
                    1 => 0,
                    _ => 0
                };

            return totCards switch
            {
                5 => 1,
                4 => 1,
                3 => 0,
                2 => 0,
                1 => 0,
                _ => 0
            };
        }



        private int MakeABet()
        {
            var totPlayers = GameManager.PlayingPlayers.Count;
            var totCards = Cards.Count;

            _winCards = new List<Card>();


            var topCards = from card in Cards where card.Value >= 32 select card;
            _winCards.AddRange(topCards);

            var threshold = GetThreshold(totCards, totPlayers);
            foreach(var card in Cards.Except(topCards))
                if (_winCards.Count >= threshold)
                    break;
                else if (card.Value >= 26)
                    _winCards.Add(card);


            var bet = _winCards.Count;
            var possibleBets = BetViewModel.PossibleBets;
            var hasPaolo = Cards.Any(card => card.IsPaolo);

            if (hasPaolo && bet != 0)
                bet -= 1;

            bet = FixBet(bet);
            bet = GetNear(bet, possibleBets, hasPaolo);
            return bet;
        }

        private int FixBet(int bet)
        {
            var otherBets = GameManager.Bets;
            var totPlayers = GameManager.PlayingPlayers.Count;
            var alfa = (double) otherBets.Count / (totPlayers - 1);
            var avg = otherBets.Any() ? otherBets.Average() : 0;
            var expectedAvg = (double) Cards.Count / totPlayers;

            var delta = (alfa) * (expectedAvg - avg);
            var offset = (int)Math.Round(delta);
            var ret = bet + offset;
            return ret <= 0 ? 0 : ret >= Cards.Count ? Cards.Count : ret;
        }

        private int MakeOneCardBet()
        {
            var bet = LastPhaseViewModel.Cards.Any(c => c.Value >= 20) ? 0 : 1;
            if (bet == 0 && !LastPhaseViewModel.CanBetLose)
                bet = 1;
            else if (bet == 1 && !LastPhaseViewModel.CanBetWin)
                bet = 0;

            return bet;
        }



        private int GetDropThreshold(int totPlayers) => totPlayers switch
        {
            2 => 0,
            3 => 1,
            4 => 1,
            5 => 2,
            6 => 3,
            _ => totPlayers/2
        };


        private List<Player> GetOrderedPlayers(IReadOnlyList<Player> allPlayers, Player phaseStarter)
        {
            int index = 0;
            for(int i=0; i < allPlayers.Count; i++)
                if (allPlayers[i] == phaseStarter)
                {
                    index = i;
                    break;
                }


            var orderedPlayers = new List<Player>();
            for(var i=0; i<allPlayers.Count; i++, index = index + 1 == allPlayers.Count? 0 : index + 1)
                orderedPlayers.Add(allPlayers[index]);

            return orderedPlayers;
        }

        private Card ChooseToDrop()
        {
            if (Cards.Count == 1)
            {
                var ret = Cards.First();
                if (ret.IsPaolo)
                    ret.Value = PhaseWin < WinBet ? 41 : -1;
                return ret;
            }

            var currentPLayer = GameManager.CurrentPlayer;
            var allPlayers = GameManager.PlayingPlayers;
            var orderedPlayers = GetOrderedPlayers(allPlayers, GameManager.PhaseStarter);


            var thisBotPosition = orderedPlayers.IndexOf(this);
            var threshold = GetDropThreshold(allPlayers.Count);

            var hasToWin = Math.Abs(WinBet.Value - PhaseWin.Value) >= Cards.Count;
            var betterLose = !hasToWin && (thisBotPosition <= threshold || PhaseWin >= WinBet);
            if (!betterLose)
                ;
            var isLastToPlay = GameManager.DroppedCards.Count == GameManager.PlayingPlayers.Count - 1;
            var notPaoloCards = (from card in Cards where !card.IsPaolo select card).ToArray();

            return betterLose ? TryLose(notPaoloCards, isLastToPlay) : TryWin(notPaoloCards);
        }



        private Card TryWin(Card[] notPaoloCards)
        {
            var possibleCards = (from card in _winCards
                orderby card.Value
                where GameManager.DroppedCards.All(c => c.Value < card.Value)
                select card).ToArray();

            return possibleCards.Any() ? possibleCards.First() : (from card in notPaoloCards orderby card.Value select card).First();
        }



        private Card TryLose(Card[] notPaoloCards, bool isLastToPlay)
        {
            var maxDropped =
                (from dropped in GameManager.DroppedCards
                    where dropped.Value == GameManager.DroppedCards.Max(c => c.Value)
                    select dropped).FirstOrDefault();

            Card toDrop;
            if (maxDropped == null)
                toDrop = (from card in notPaoloCards orderby card.Value select card).FirstOrDefault();
            else
            {
                var maxToDrop = (from c in notPaoloCards.Except(_winCards)
                    orderby c.Value descending
                    where c.Value < maxDropped.Value
                    select c).FirstOrDefault();

                if (maxToDrop != null)
                    toDrop = maxToDrop;
                else if (isLastToPlay)
                    toDrop = (from card in notPaoloCards
                        where card.Value == notPaoloCards.Max(c => c.Value)
                        select card).FirstOrDefault();
                else
                    toDrop = (from card in notPaoloCards
                        orderby card.Value 
                        where card.Value > maxDropped.Value
                        select card).FirstOrDefault();
            }

        

            return toDrop;
        }
    }
}
