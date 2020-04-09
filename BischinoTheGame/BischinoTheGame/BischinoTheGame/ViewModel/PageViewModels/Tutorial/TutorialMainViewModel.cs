using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels.Tutorial
{
    public class TutorialMainViewModel : PageViewModel
    {
        public event Func<string, Task> AutoWriteText;
        public event Func<string, Task> FadeText;
        public event Func<Task> WelcomeCompleted;
        public event Func<Task> FadeOrderedCards;
        public event Func<Task> FadeSeeds;
        public event Func<Task> ShowFirstPaolo;
        public event Func<Task> DuplicatePaolo;
        public event Func<Task> MaximumPaolo;
        public event Func<Task> MinimumPaolo;
        public event Func<Task> PaoloCompleted;
        public event Func<Task> FadeLives;
        public event Func<Task> DropCards;
        public event Func<Task> YourTurn;
        public event Func<Task> DropTheCard;
        public event Func<Task> AddPoint;
        public event Func<Task> Restart;
        public event Func<Task> StrongCardsPopup;
        public event Func<Task> PaoloPopup;
        public event Func<Task> ShowPrediction;
        public event Func<Task> ShowOtherPlayersPredictions;
        public event Func<Task> FadeOtherPlayersPrediction;
        public event Func<Task> PredictionWinComparison;
        public event Func<Task> LooseALife;
        public event Func<Task> AnimateSetupRound2;
        public event Func<Task> LastRound;

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


        public async Task Start()
        {
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
            await FadeText.Invoke("Welcome to Bischino");
            await Task.Delay(4000);
            /*
            await FadeText.Invoke("Ready to play?");
            await Task.Delay(2000);
            */
            await FadeText.Invoke("Let's start");

            await WelcomeCompleted.Invoke();
        }
        private async Task OnWelcomeCompleted()
        {
            await FadeText("In this game, all the cards are ordered by value");
            await Task.Delay(2500);
            await FadeText("From ace to king");
            await Task.Delay(1000);
            await FadeOrderedCards();

            await FadeText("and in order of suit");
            await Task.Delay(1500);
            await FadeSeeds();
        }
        private async Task PaoloTime()
        {
            await FadeText("But there is an exception");
            await ShowFirstPaolo();
            await Task.Delay(1000);
            await FadeText("This card can have two different values");
            await Task.Delay(1500);
            await DuplicatePaolo();
            await FadeText("Maximum");
            await MaximumPaolo();
            await FadeText("and minimum");
            await MinimumPaolo();
            await Task.Delay(1000);
            await PaoloCompleted();

            await Task.Delay(1000);
        }
        private async Task LivesTime()
        {
            /*
            await FadeText("Let's now talk about lives");
            await Task.Delay(1500);
            await FadeText("Each player has 3 lives");
            await Task.Delay(1500);
            */
            await FadeLives();
            /*
            await Task.Delay(1000);
            await FadeText("That's it");
            await Task.Delay(500);
            */
        }
        private async Task DropTime()
        {
            await FadeText("Okay, now let's assume you're playing with 2 players");
            await Task.Delay(2500);
            await FadeText("Every player must play one card per turn");
            await Task.Delay(2000);
            await DropCards();
            DropCommand = new Command(_ => OnDrop());
            await YourTurn();
        }



        public async void OnDrop()
        {
            IsPageEnabled = false;
            DropCommand = null;
            IsPageEnabled = true;
            await DropTheCard();
            await FadeText("Great");
            await Task.Delay(500);
            await FadeText("Because your card has the higher value, you gain a point");
            await AddPoint();
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

            await FadeText("The aim of the game is to guess the number of points you will gain");
            await Task.Delay(2000);
            /*
            await FadeText("So let's restart the game");
            */
            await Restart();
            await FadeText("This is more or less how you have to think");
            await Task.Delay(2000);
            await FadeText("These cards have an high value");
            await StrongCardsPopup();
            await Task.Delay(2000);
            await FadeText("So you will probably win on 2 turns");
            await Task.Delay(4000);
            await FadeText("Remember you also have the special card");
            await PaoloPopup();
            /*
            await Task.Delay(5000);
            await FadeText("When you drop it you can choose it to be the highest or lowest card of the whole game!");
            */
            await Task.Delay(3000);
            await FadeText("So a good guess can be 2");
            await ShowPrediction();
            await Task.Delay(3000);
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
            await FadeText("Okay, but how can you lose one of your lives?");
            await Task.Delay(2000);
            await FadeText("Well, when all the players drop their 5 cards down");
            await Task.Delay(3000);
            await FadeText("The game compares your guess and your actual wins");
            await Task.Delay(1000);
            await PredictionWinComparison();
            CurrentWin = 3;
            await Task.Delay(5000);
            /*
            await FadeText("Suppose your wins are 3");
            await Task.Delay(3000);
            
            await FadeText("You have predicted 2, but you actually won 3");
            await Task.Delay(4000);
            */
            await FadeText("You are off by one, so you lose a life");
            await LooseALife();
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
            await FadeText("You are now ready for the next round");
            await Task.Delay(2000);
            await FadeText("The deck is shuffled, you have 4 cards in you hands, and you are ready to make another bet");
            CurrentWin = 0;
            Bet = null;
            await AnimateSetupRound2();
            await Task.Delay(3000);
        }
        private async Task OneCardRoundTime()
        {
            await FadeText("One last thing");
            await Task.Delay(3000);
            await FadeText("The round with only one card is different");
            await Task.Delay(3000);
            await FadeText("You can't see your card, but you can see the cards of all other players");
            await Task.Delay(3000);
            await FadeText("and on these you have to base your guess");
        }
        private async Task EndTutorial()
        {
            await Task.Delay(5000);
            await FadeText("Now you are ready to play");
            await Task.Delay(2000);
            AppController.Settings.FirstRun = false;
            await AppController.Navigation.TutorialNavigation.NotifyTutorialEnded();
        }
       

        private void SetupProperties()
        {
            Paolo = AppController.Settings.GetCardIcon("30");
            Lives = 3;
            Dropped1 = AppController.Settings.GetCardIcon("5");
            Dropped2 = AppController.Settings.GetCardIcon("17");
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
    }
}
