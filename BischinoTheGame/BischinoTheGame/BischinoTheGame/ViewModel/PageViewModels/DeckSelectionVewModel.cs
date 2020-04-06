using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using BischinoTheGame.Model.Settings;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class DeckSelectionVewModel : PageViewModel
    {
        private const int DeckSize = 40;

        public readonly ObservableCollection<IList<string>> Decks = new ObservableCollection<IList<string>>();

        
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        private IList<string> _deck1;
        public IList<string> Deck1
        {
            get => _deck1;
            set => SetProperty(ref _deck1, value);
        }


        private IList<string> _deck2;
        public IList<string> Deck2
        {
            get => _deck2;
            set => SetProperty(ref _deck2, value);
        }


        private IList<string> _deck3;
        public IList<string> Deck3
        {
            get => _deck3;
            set => SetProperty(ref _deck3, value);
        }


        private Command _deck1Command;
        public Command Deck1Command
        {
            get => _deck1Command;
            set => SetProperty(ref _deck1Command, value);
        }


        private Command _deck2Command;
        public Command Deck2Command
        {
            get => _deck2Command;
            set => SetProperty(ref _deck2Command, value);
        }


        private Command _deck3Command;
        public Command Deck3Command
        {
            get => _deck3Command;
            set => SetProperty(ref _deck3Command, value);
        }


        public DeckSelectionVewModel()
        {
            Name = "dsadsa";
            LoadDecks();
            
        }

        private void LoadDecks()
        {
            Decks.Clear();
            foreach (var type in (DeckType[]) Enum.GetValues(typeof(DeckType)))
            {
                var deck = new List<string>();
                for(int i=0; i < DeckSize; i++)
                    deck.Add(AppController.Settings.GetCardIcon($"{i}", type));
                Decks.Add(deck);
            }

            Deck1 = Decks[0];
            Deck1Command = new Command(_ => ChooseDeck(DeckType.A));
            
            Deck2 = Decks[1];
            Deck2Command = new Command(_ => ChooseDeck(DeckType.B));


            Deck3 = Decks[2];
            Deck3Command = new Command(_ => ChooseDeck(DeckType.C));
        }

        private async void ChooseDeck(DeckType type)
        {
            AppController.Settings.DeckType = type;
            await AppController.Navigation.DisplayAlert("Information", "Saved");
            await AppController.Navigation.RoomNavigation.NotifyDeckChosen();
        }
    }
}