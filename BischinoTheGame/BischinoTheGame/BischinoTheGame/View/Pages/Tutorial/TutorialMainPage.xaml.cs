using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Model;
using BischinoTheGame.View.Extensions;
using BischinoTheGame.ViewModel.PageViewModels.Tutorial;
using Rooms.Controller;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BischinoTheGame.View.Pages.Tutorial 
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TutorialMainPage : ContentPage, ITutorial
    {
        private TutorialMainViewModel _viewModel;
        private const int FadeTime = 500;
        private bool isStarted;



        private List<Xamarin.Forms.View> Cards { get; set; }
        private List<ImageButton> OrderedCards { get; set; }
        private List<ImageButton> Kings { get; set; }



        public TutorialMainPage()
        {
            InitializeComponent();
        }
        


        protected override void OnDisappearing()
        {
            MessagingCenter.Send(this, ViewMessagingConstants.Unspecified);
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, ViewMessagingConstants.Landscape);

            if (isStarted)
                return;
            
            if (!(BindingContext is TutorialMainViewModel vm)) 
                throw new Exception("Binding context must be tutorial view model");

            isStarted = true;

            Cards = new List<Xamarin.Forms.View> {Card0, Card1, Card2, Card3, Card4};
            OrderedCards = new List<ImageButton> { ACard0, ACard1, ACard2, ACard3, ACard4, ACard5, ACard6, ACard7, ACard8, ACard9};
            Kings = new List<ImageButton> {King0, King1, King2, King3};

            _viewModel = vm;

            await _viewModel.Start(this);
        }





        public async Task LastRound()
        {
            var fadeTasks = from card in Cards select card.FadeTo(0, FadeTime);
            await Task.WhenAll(fadeTasks);
        }


        public async Task AnimateRound2()
        {
            await WinLabel1.ScaleTo(2, FadeTime / 2);
            await WinLabel1.ScaleTo(1, FadeTime / 2);

            await Card4.FadeTo(0, FadeTime);
            Card4.IsVisible = false;
            var fadeOffTasks = from card in Cards select card.FadeTo(0, FadeTime);
            await Task.WhenAll(fadeOffTasks);
            var fadeOnTasks = from card in Cards where card != Card4 select card.FadeTo(1, FadeTime);
            await Task.WhenAll(fadeOnTasks);
        }


        public async Task FadeOtherPlayersPrediction()
        {
            await Task.WhenAll(Player1Bet.ScaleTo(0, FadeTime / 3), Player2Bet.ScaleTo(0, FadeTime / 3));
        }


        public async Task LoseALife()
        {
            _viewModel.Lives = 2;
            await LivesLabel1.ScaleTo(2, FadeTime);
            await LivesLabel1.ScaleTo(1, FadeTime);
            await Task.Delay(2000);
            await Task.WhenAll(
                WinLabel1.TranslateTo(0, 00, FadeTime , Easing.BounceOut), WinLabel2.TranslateTo(0, 0, FadeTime, Easing.BounceOut),
                BetLabel1.TranslateTo(0, 0, FadeTime, Easing.BounceOut), BetLabel2.TranslateTo(0, 0, FadeTime, Easing.BounceOut));
        }


        public async Task PredictionWinComparison()
        {
            var tasks = from card in Cards select card.FadeTo(0, FadeTime);
            await Task.WhenAll(tasks);

            await Task.WhenAll(WinLabel1.TranslateTo(0, -50, FadeTime / 2, Easing.BounceOut), WinLabel2.TranslateTo(0, -50, FadeTime / 2, Easing.BounceOut),
                BetLabel1.TranslateTo(0, -50, FadeTime / 2, Easing.BounceOut), BetLabel2.TranslateTo(0, -50, FadeTime / 2, Easing.BounceOut));

            await Task.WhenAll(WinLabel1.TranslateTo(-140, -50, FadeTime / 2, Easing.BounceOut), WinLabel2.TranslateTo(-140, -50, FadeTime / 2, Easing.BounceOut),
                BetLabel1.TranslateTo(250, -50, FadeTime / 2, Easing.BounceOut), BetLabel2.TranslateTo(250, -50, FadeTime / 2, Easing.BounceOut));
        }


        public async Task ShowOtherPlayersPredictions()
        {
            await Player1Bet.ScaleTo(1, FadeTime / 3);

            AppController.AudioManager.PlaySound(SoundEffect.Pop);
            await Task.Delay(1200);

            await Player2Bet.ScaleTo(1, FadeTime / 3);
            AppController.AudioManager.PlaySound(SoundEffect.Pop);
            await Task.Delay(1200);
        }


        public async Task ShowPrediction()
        {
            await Card4.TranslateTo(0, 0, FadeTime / 3);
            await BetLabel2.ScaleTo(1, FadeTime / 3);
            await BetLabel1.ScaleTo(1, FadeTime / 3);
        }


        public async Task PaoloPopup()
        {
            await Task.WhenAll(Card2.TranslateTo(0, 0, FadeTime / 3), Card3.TranslateTo(0, 0, FadeTime / 3));
            await Card4.TranslateTo(0, -50, FadeTime / 3);
        }


        public async Task StrongCardsPopup()
        {
            await Task.WhenAll(Card2.TranslateTo(0, -50, FadeTime / 3), Card3.TranslateTo(0, -50, FadeTime / 3));
        }


        public async Task Restart()
        {
            await Task.WhenAll(Card3.ScaleTo(1, FadeTime, Easing.BounceOut), Card3.TranslateTo(0, 0, FadeTime));
            
            _viewModel.CurrentWin = 0;
            await WinLabel1.ScaleTo(1.2, FadeTime / 3);
            await WinLabel1.ScaleTo(1, FadeTime / 3);
        }


        public async Task AddPoint()
        {
            await WinLabel2.ScaleTo(1, FadeTime / 3);
            await WinLabel1.ScaleTo(1, FadeTime / 3);
        }


        public async Task DropTheCard()
        {
            await Card3.ScaleTo(0, FadeTime / 3);
            AppController.AudioManager.PlaySound(SoundEffect.Pop);
            await Dropped3.ScaleTo(1, FadeTime / 3);
            await Task.Delay(3000);
            await DroppedStackLayout.FadeTo(0, FadeTime);
        }


        public async Task YourTurn()
        {
            await Card3.TranslateTo(0, -30, 600, Easing.BounceOut);
        }


        public async Task DropCards()
        {
            await TutorialLabel.FadeTo(0, FadeTime);
            await Dropped1.ScaleTo(1, FadeTime / 3);
            AppController.AudioManager.PlaySound(SoundEffect.Pop);
            await Task.Delay(500);
            await Dropped2.ScaleTo(1, FadeTime / 3);
            AppController.AudioManager.PlaySound(SoundEffect.Pop);
        }


        public async Task FadeLives()
        {
            await LivesLabel2.ScaleTo(1, FadeTime/3);
            await LivesLabel1.ScaleTo(1, FadeTime/3);
        }


        public async Task PaoloCompleted()
        {
            await Task.WhenAll(Paolo2Box.FadeTo(0, FadeTime), Paolo1Box.FadeTo(0, FadeTime));
            Paolo1Box.IsVisible = Paolo2Box.IsVisible = false;
            await Paolo2.TranslateTo(0, 0, 600, Easing.BounceOut);
            await Card4.TranslateTo(0, 0, 1000, Easing.BounceOut);
            var tasks = from card in Cards where card != Card4 select card.FadeTo(1, FadeTime);
            await Task.WhenAll(tasks);
        }


        public async Task MinimumPaolo()
        {
            Paolo2Box.Opacity = 0;
            Paolo2Box.IsVisible = true;
            await Paolo2Box.FadeTo(1);
            await Paolo2Box.ScaleTo(1.5, FadeTime, Easing.BounceOut);
            await Paolo2Box.ScaleTo(1);
        }


        public async Task MaximumPaolo()
        {
            Paolo1Box.Opacity = 0;
            Paolo1Box.IsVisible = true;
            await Paolo1Box.FadeTo(1);
            await Paolo1Box.ScaleTo(1.5, FadeTime, Easing.BounceOut);
            await Paolo1Box.ScaleTo(1);
        }


        public async Task DuplicatePaolo()
        {
            var tasks = from card in Cards where card != Card4 select card.FadeTo(0, FadeTime);
            await Task.WhenAll(tasks);
            await Paolo2.TranslateTo(-300, 0, 600, Easing.BounceOut);
        }


        public async Task ShowFirstPaolo()
        {
            await Card4.TranslateTo(0, -50, 1000, Easing.BounceOut);
        }


        public async Task FadeSeeds()
        {
            await TutorialLabel.FadeTo(0, FadeTime);

            var height = SeedsStackLayout.HeightRequest;
            var unit = height / (Kings.Count + 1);
            var scale = unit;

            foreach (var king in Kings)
                king.HeightRequest = scale += unit;

            foreach (var seed in Kings)
                await seed.ScaleTo(1, FadeTime / 3);

            await Task.Delay(3000);
            Kings.Reverse();
            foreach (var card in Kings)
                await card.ScaleTo(0, FadeTime / 3);
        }


        public async Task FadeOrderedCards()
        {
            OrderedCardsScrollView.IsVisible = OrderedCardsStackLayout.IsVisible = true;

            await TutorialLabel.FadeTo(0, FadeTime);
            var height = OrderedCardsStackLayout.HeightRequest;
            var unit = height / ( OrderedCards.Count + 1) ;
            var scale = unit;

            foreach (var card in OrderedCards)
                card.HeightRequest = scale += unit;

            foreach(var card in OrderedCards)
                await card.ScaleTo(1, FadeTime / 5);

            await Task.Delay(2000);
            OrderedCards.Reverse();
            foreach(var card in OrderedCards)
                await card.ScaleTo(0, FadeTime / 6);

            OrderedCardsStackLayout.IsVisible = false;
        }


        public async Task WelcomeCompleted()
        {
            await TutorialLabel.FadeTo(0, FadeTime);
            await Card0.FadeTo(1,FadeTime);
            foreach (var card in Cards)
                await card.ScaleTo(1, FadeTime/3);
            TutorialLabel.FontSize = 19;
        }


        public async Task WriteText(string text)
        {
            await TutorialLabel.FadeTo(0, FadeTime);
            _viewModel.TutorialText = text;
            await TutorialLabel.FadeTo(1, FadeTime);
        }


        public Task AutoWriteText(string text)
            => this.AutomateText(text, val => _viewModel.TutorialText = val, 60, Easing.CubicIn);
    }
}