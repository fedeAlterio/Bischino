using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Model.Settings;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class DeckSelectionVewModel : PageViewModel
    {
        private const int DeckSize = 40;
        private readonly ObservableCollection<IList<string>> _decks = new();


        // Initialization
        public DeckSelectionVewModel()
        {
            Name = "dsadsa";
            LoadDecks();
            Deck1 = new (_deck1);
            Deck2 = new (_deck2);
            Deck3 = new  (_deck3);
        }

        private void LoadDecks()
        {
            _decks.Clear();
            foreach (var type in (DeckType[])Enum.GetValues(typeof(DeckType)))
            {
                var deck = new List<string>();
                for (int i = 0; i < DeckSize; i++)
                    deck.Add(AppController.Settings.GetCardIcon($"{i}", type));
                _decks.Add(deck);
            }

            IAsyncCommand ChooseDeckCommand(DeckType deckType) => NewCommand(async () => await ChooseDeck(deckType));
            _deck1.AddRange(_decks[0]);
            Deck1Command = ChooseDeckCommand(DeckType.A);

            _deck2.AddRange(_decks[0]);
            Deck2Command = ChooseDeckCommand(DeckType.B);

            _deck3.AddRange(_decks[0]);
            Deck3Command = ChooseDeckCommand(DeckType.C);
        }


        // Commands
        public IAsyncCommand Deck1Command { get; private set; }
        public IAsyncCommand Deck2Command { get; private set; }
        public IAsyncCommand Deck3Command { get; private set; }



        // Properties
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        private ObservableRangeCollection<string> _deck1 = new();
        public ReadOnlyObservableCollection<string> Deck1 { get; }

        private ObservableRangeCollection<string> _deck2 = new();
        public ReadOnlyObservableCollection<string> Deck2 { get; }

        private ObservableRangeCollection<string> _deck3 = new();
        public ReadOnlyObservableCollection<string> Deck3 { get; }



        // Commands Handlers
        private async Task ChooseDeck(DeckType type)
        {
            AppController.Settings.DeckType = type;
            await AppController.Navigation.DisplayAlert("Information", "Saved");
            await AppController.Navigation.GameNavigation.NotifyDeckChosen();
        }
    }
}