using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Rooms.Controller;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RulesViewModel : PageViewModel
    {
        private const int DeckSize = 40;


        private ObservableCollection<string> _deck = new ObservableCollection<string>();
        public ObservableCollection<string> Deck
        {
            get => _deck;
            set => SetProperty(ref _deck, value);
        }


        private string _paolo;
        public string Paolo
        {
            get => _paolo;
            set => SetProperty(ref _paolo, value);
        }

        public RulesViewModel()
        {
            LoadDecks();
        }

        public void LoadDecks()
        {
            Deck.Clear();
            for (int i = 0; i < DeckSize; i++)
                if(i != 30)
                    Deck.Add(AppController.Settings.GetCardIcon($"{i}"));
            Paolo = AppController.Settings.GetCardIcon("30");
        }
    }
}
