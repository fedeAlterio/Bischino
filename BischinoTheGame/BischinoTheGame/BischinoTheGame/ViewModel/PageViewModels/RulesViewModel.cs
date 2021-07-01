using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class RulesViewModel : PageViewModel
    {
        private const int DeckSize = 40;

        // Initialization
        public RulesViewModel()
        {
            LoadDecks();
            TutorialCommand = NewCommand(ToTutorial);
            Deck = new(_deck);
        }


        // Commands
        public IAsyncCommand TutorialCommand { get; }


        // Properties
        private ObservableCollection<string> _deck = new();
        public ReadOnlyObservableCollection<string> Deck { get; }


        private string _paolo;
        public string Paolo
        {
            get => _paolo;
            set => SetProperty(ref _paolo, value);
        }




       

        // Commands Handlers
        private async Task ToTutorial()
        {
            await AppController.Navigation.TutorialNavigation.ToMainPage();
        }


        // Helpers
        public void LoadDecks()
        {
            _deck.Clear();
            for (int i = 0; i < DeckSize; i++)
                if(i != 30)
                    _deck.Add(AppController.Settings.GetCardIcon($"{i}"));
            Paolo = AppController.Settings.GetCardIcon("30");
        }
    }
}
