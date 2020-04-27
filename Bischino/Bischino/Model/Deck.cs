using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Extensions;

namespace Bischino.Model 
{ 
    public class Deck
    {
        private IList<Card> Cards { get; set; }

        public Deck()
        {
            Shuffle();
        }

        public void Shuffle()
        {
            var cards = GetAll();
            cards.Shuffle();
            Cards = cards;
        }


        public IList<Card> Draw(int amount, string owner)
        {
            var cards = new List<Card>();
            for(int i=0; i < amount; i++)
            { 
                cards.Add(Cards[0]);
                Cards[0].Owner = owner;
                Cards.RemoveAt(0);
            }

            return cards;
        }


        public static IList<Card> GetAll()
        {
            var cards = new List<Card>();
            /*
            for (int i = 0; i < 40; i++)
                cards.Add(new Card
                {
                    IsPaolo = true,
                    Name = $"{30}",
                    Value = 30
                });
            */
            for (int i=0; i < 40; i++)
                cards.Add(new Card
                    {
                        IsPaolo = i==30,
                        Name = $"{i}",
                        Value = i
                    });
        
            return cards;
        }
    }
}
