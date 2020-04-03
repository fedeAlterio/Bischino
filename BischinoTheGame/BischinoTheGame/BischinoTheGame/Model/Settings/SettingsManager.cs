using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.Model.Settings
{
    public class SettingsManager
    {
        public DeckType DeckType { get; set; } = DeckType.A;

        public string GetCardIcon(int number) => GetCardIcon(number, DeckType);

        public string GetCardIcon(int number, DeckType deckType) => deckType switch
        {
            DeckType.A => $"c{number}",
            DeckType.B => $"D{number}"
        };
}
}
