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

        public readonly ObservableCollection<IList<string>> Decks = new ObservableCollection<IList<string>>();


        // Initialization
        public DeckSelectionVewModel()
        {
            Name = "dsadsa";
            LoadDecks();
            Deck1 = new ReadOnlyObservableCollection<string>(_deck1);
            Deck2 = new ReadOnlyObservableCollection<string>(_deck2);
            Deck3 = new ReadOnlyObservableCollection<string>(_deck3);
        }

        private void LoadDecks()
        {
            Decks.Clear();
            foreach (var type in (DeckType[])Enum.GetValues(typeof(DeckType)))
            {
                var deck = new List<string>();
                for (int i = 0; i < DeckSize; i++)
                    deck.Add(AppController.Settings.GetCardIcon($"{i}", type));
                Decks.Add(deck);
            }

            _deck1.AddRange(Decks[0]);
            Deck1Command = NewCommand(async () => await ChooseDeck(DeckType.A));

            _deck2.AddRange(Decks[0]);
            Deck2Command = NewCommand(async () => await ChooseDeck(DeckType.B));

            _deck3.AddRange(Decks[0]);
            Deck3Command = NewCommand(async () => await ChooseDeck(DeckType.C));
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


        private ObservableRangeCollection<string> _deck1 = new ObservableRangeCollection<string>();
        public ReadOnlyObservableCollection<string> Deck1 { get; }

        private ObservableRangeCollection<string> _deck2 = new ObservableRangeCollection<string>();
        public ReadOnlyObservableCollection<string> Deck2 { get; }

        private ObservableRangeCollection<string> _deck3 = new ObservableRangeCollection<string>();
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