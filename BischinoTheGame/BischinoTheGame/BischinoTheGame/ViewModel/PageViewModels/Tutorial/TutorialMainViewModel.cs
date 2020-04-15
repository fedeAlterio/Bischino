using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels.Tutorial
{
    public class TutorialMainViewModel : PageViewModel
    {
        private ITutorial _tutorial;

        #region BottomCards

        private string _card0;
        public string Card0
        {
            get => _card0;
            set => SetProperty(ref _card0, value);
        }


        private string _card1;
        public string Card1
        {
            get => _card1;
            set => SetProperty(ref _card1, value);
        }


        private string _card2;
        public string Card2
        {
            get => _card2;
            set => SetProperty(ref _card2, value);
        }


        private string _card3;
        public string Card3
        {
            get => _card3;
            set => SetProperty(ref _card3, value);
        }


        #endregion

        #region OrderedCards

        private string _aCard0;
        public string ACard0
        {
            get => _aCard0;
            set => SetProperty(ref _aCard0, value);
        }


        private string _aCard1;
        public string ACard1
        {
            get => _aCard1;
            set => SetProperty(ref _aCard1, value);
        }


        private string _aCard2;
        public string ACard2
        {
            get => _aCard2;
            set => SetProperty(ref _aCard2, value);
        }


        private string _aCard3;
        public string ACard3
        {
            get => _aCard3;
            set => SetProperty(ref _aCard3, value);
        }


        private string _aCard4;
        public string ACard4
        {
            get => _aCard4;
            set => SetProperty(ref _aCard4, value);
        }


        private string _aCard5;
        public string ACard5
        {
            get => _aCard5;
            set => SetProperty(ref _aCard5, value);
        }


        private string _aCard6;
        public string ACard6
        {
            get => _aCard6;
            set => SetProperty(ref _aCard6, value);
        }


        private string _aCard7;
        public string ACard7
        {
            get => _aCard7;
            set => SetProperty(ref _aCard7, value);
        }


        private string _aCard8;
        public string ACard8
        {
            get => _aCard8;
            set => SetProperty(ref _aCard8, value);
        }

        private string _aCard9;
        public string ACard9
        {
            get => _aCard9;
            set => SetProperty(ref _aCard9, value);
        }

        #endregion

        #region Kings


        private string _king1;
        public string King1
        {
            get => _king1;
            set => SetProperty(ref _king1, value);
        }


        private string _king2;
        public string King2
        {
            get => _king2;
            set => SetProperty(ref _king2, value);
        }


        private string _king3;
        public string King3
        {
            get => _king3;
            set => SetProperty(ref _king3, value);
        }


        private string _king4;
        public string King4
        {
            get => _king4;
            set => SetProperty(ref _king4, value);
        }
        #endregion

        #region OtherProperies

        private string _tutorialText;
        public string TutorialText
        {
            get => _tutorialText;
            set => SetProperty(ref _tutorialText, value);
        }


        private string _paolo;
        public string Paolo
        {
            get => _paolo;
            set => SetProperty(ref _paolo, value);
        }


        private int _lives;
        public int Lives
        {
            get => _lives;
            set => SetProperty(ref _lives, value);
        }


        private string _dropped1;
        public string Dropped1
        {
            get => _dropped1;
            set => SetProperty(ref _dropped1, value);
        }


        private string _dropped2;
        public string Dropped2
        {
            get => _dropped2;
            set => SetProperty(ref _dropped2, value);
        }


        private Command _dropCommand;
        public Command DropCommand
        {
            get => _dropCommand;
            set => SetProperty(ref _dropCommand, value);
        }


        private int _currentWin;
        public int CurrentWin
        {
            get => _currentWin;
            set => SetProperty(ref _currentWin, value);
        }


        private int? _bet;
        public int? Bet
        {
            get => _bet;
            set => SetProperty(ref _bet, value);
        }

        #endregion


        public async Task Start(ITutorial tutorial)
        {
            _tutorial = tutorial;
            SetupBottomCards();
            SetupCentralCards();
            SetupKings();
            SetupProperties();

            await PrintWelcome();
            await OnWelcomeCompleted();
            await PaoloTime();
            await LivesTime();
            await DropTime();
        }



        private async Task PrintWelcome()
        {
            await WriteAndAwait("Welcome to Bischino", 4000);
            /*
            await FadeText.Invoke("Ready to play?");
            await Task.Delay(2000);
            */
            await WriteText("Let's start");
            await _tutorial.WelcomeCompleted();
        }
        private async Task OnWelcomeCompleted()
        {
            await WriteAndAwait("In this game, all the cards are ordered in strength", 2500);
            await WriteAndAwait("From ace to king", 1000);
            await _tutorial.FadeOrderedCards();

            await WriteAndAwait("and ordered in suit", 1500);
            await _tutorial.FadeSeeds();
        }
        private async Task PaoloTime()
        {
            await WriteText("But there is an exception");
            await _tutorial.ShowFirstPaolo();
            await Task.Delay(1000);

            await WriteAndAwait("This card can have two different values", 1500);
            await _tutorial.DuplicatePaolo();
            await WriteText("highest");
            await _tutorial.MaximumPaolo();
            await WriteText("and lowest");
            await _tutorial.MinimumPaolo();
            await Task.Delay(1000);
            await WriteAndAwait("in the game", 1300);

            await _tutorial.PaoloCompleted();

            await Task.Delay(1000);
        }
        private async Task LivesTime()
        {
            await _tutorial.FadeLives();
        }
        private async Task DropTime()
        {
            await WriteAndAwait("Okay, now let's assume you're playing with 2 players", 2500);
            await WriteAndAwait("Every player must play one card at time", 2000);
            await _tutorial.DropCards();
            DropCommand = new Command(_ => OnDrop());
            await _tutorial.YourTurn();
        }



        public async void OnDrop()
        {
            IsPageEnabled = false;
            DropCommand = null;
            IsPageEnabled = true;
            await _tutorial.DropTheCard();
            await WriteAndAwait("Great", 500);
            
            await WriteText("Because your card has an higher rank, you are awarded a point");
            await _tutorial.AddPoint();
            await Task.Delay(2000);

            await BetTime();
            await OnLivesCalculations();
            await OnRound2();
            await OneCardRoundTime();
            await EndTutorial();
        }



        private async Task BetTime()
        {
            /*
            await FadeText("however, earning points isn't always a good thing");
            await Task.Delay(2000);
            */

            await WriteAndAwait("The aim of the game is to guess the number of tricks you'll win in a hand", 3000);
            /*
            await FadeText("So let's restart the game");
            */
            await _tutorial.Restart();
            await WriteAndAwait("This is more or less how you have to think", 2000);
            await WriteText("These cards have an high value");
            await _tutorial.StrongCardsPopup();
            await Task.Delay(2000);
            await WriteAndAwait("So you will probably win on 2 tricks", 4000);
            await WriteText("Remember you also have the special card");
            await _tutorial.PaoloPopup();
            /*
            await Task.Delay(5000);
            await FadeText("When you drop it you can choose it to be the highest or lowest card of the whole game!");
            */
            await Task.Delay(3000);
            await WriteText("So a good prediction can be 2");
            await _tutorial.ShowPrediction();
            await Task.Delay(2000);
            /*
            await FadeText("There is one more thing about this phase");
            await Task.Delay(2000);
            await FadeText("The last player to make his bet has an extra constraint");
            await Task.Delay(2000);
            await FadeText("The sum of his prediction, added to other players guesses, cannot be equals to the number of cards in his hands");
            await Task.Delay(8000);
            await FadeText("Let's make it easy with an example");
            await Task.Delay(2000);
            await FadeText("Let's say you are the last of three players to bet");
            await Task.Delay(2000);
            await FadeText("And let's say these are their predictions");
            await ShowOtherPlayersPredictions();
            await Task.Delay(2000);
            await FadeText("We have 5 cards. The rule says that the guesses sum cannot be equal to 5");
            await Task.Delay(7000);
            await FadeText("So you can't bet 2, because 2 + 1 + 2 = 5");
            await Task.Delay(5000);

            await FadeOtherPlayersPrediction();
            */
        }
        private async Task OnLivesCalculations()
        {
            await WriteAndAwait("Okay, but how can you lose one of your lives?", 2000);
            await WriteAndAwait("Well, when all the players play all their 5 cards", 3000);
            await WriteAndAwait("The game compares your bet and the number of points you won", 1000);
            await _tutorial.PredictionWinComparison();
            CurrentWin = 3;
            await Task.Delay(5000);
            /*
            await FadeText("Suppose your wins are 3");
            await Task.Delay(3000);
            
            await FadeText("You have predicted 2, but you actually won 3");
            await Task.Delay(4000);
            */
            await WriteText("The difference is one, so you lose a life");
            await _tutorial.LoseALife();
            await Task.Delay(2000);
            /*
            await Task.Delay(2000);
            await FadeText("If your lives go to 0, you have lost the game");
            */
           
            //await Task.Delay(2000);
        }
        private async Task OnRound2()
        {
            Card0 = AppController.Settings.GetCardIcon("13");
            Card1 = AppController.Settings.GetCardIcon("21");
            Card2 = AppController.Settings.GetCardIcon("10");
            Card3 = AppController.Settings.GetCardIcon("31");
            await WriteAndAwait("You are now ready for the next round", 2000);
            await WriteText("The deck is shuffled, you have 4 cards in your hand, and you have to make another bet");
            CurrentWin = 0;
            Bet = null;
            await _tutorial.AnimateRound2();
            await Task.Delay(3000);
        }
        private async Task OneCardRoundTime()
        {
            await WriteAndAwait("One last thing", 2000);
            await WriteAndAwait("The one-card hand has different rules", 2000);
            await WriteAndAwait("You can't see your card, but you can see the cards of all other players", 3000);
            await WriteAndAwait("Your bet will be based on the cards you see in your opponents' hands", 3000);
        }
        private async Task EndTutorial()
        {
            await WriteAndAwait("Now you are ready to play", 2000);
            await AppController.Navigation.TutorialNavigation.NotifyTutorialEnded();
        }
       

        private void SetupProperties()
        {
            Paolo = AppController.Settings.GetCardIcon("30");
            Dropped1 = AppController.Settings.GetCardIcon("5");
            Dropped2 = AppController.Settings.GetCardIcon("17");
            Lives = 3;
            CurrentWin = 1;
            Bet = 2;
        }
        private void SetupKings()
        {
            King1 = AppController.Settings.GetCardIcon("9");
            King2 = AppController.Settings.GetCardIcon("19");
            King3 = AppController.Settings.GetCardIcon("29");
            King4 = AppController.Settings.GetCardIcon("39");
        }
        private void SetupCentralCards()
        {
            ACard0 = AppController.Settings.GetCardIcon("0");
            ACard1 = AppController.Settings.GetCardIcon("1");
            ACard2 = AppController.Settings.GetCardIcon("2");
            ACard3 = AppController.Settings.GetCardIcon("3");
            ACard4 = AppController.Settings.GetCardIcon("4");
            ACard5 = AppController.Settings.GetCardIcon("5");
            ACard6 = AppController.Settings.GetCardIcon("6");
            ACard7 = AppController.Settings.GetCardIcon("7");
            ACard8 = AppController.Settings.GetCardIcon("8");
            ACard9 = AppController.Settings.GetCardIcon("9");
        }
        private void SetupBottomCards()
        {
            Card0 = AppController.Settings.GetCardIcon("3");
            Card1 = AppController.Settings.GetCardIcon("10");
            Card2 = AppController.Settings.GetCardIcon("22");
            Card3 = AppController.Settings.GetCardIcon("33");
        }
        

        private async Task WriteAndAwait(string text, int delay)
        {
            await _tutorial.WriteText(text);
            await Task.Delay(delay);
        }

        private Task WriteText(string text) => _tutorial.WriteText(text);
    }
}
