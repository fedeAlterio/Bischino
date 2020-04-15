using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ImageButton = Xamarin.Forms.ImageButton;

namespace BischinoTheGame.View.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        public const double PlayerCardWidth = 400 / 5.0;
        private GameViewModel _viewModel;
        private ImageButton _clickedImageButton;
        private bool _isInitialized;

        public Task<bool> DeletingCardAnimation { get; set; }




        public GamePage() => InitializeComponent();




        private void SetupEvents()
        {
            _viewModel.PlayerCardsUpdated += ViewModel_PlayerCardsUpdated;
            _viewModel.DroppedCardsUpdated += ViewModel_DroppedCardsUpdated;
            _viewModel.YourTurn += ViewModel_YourTurnEventHandler;
            _viewModel.NewMatchSnapshot += ViewModel_NewMatchSnapshot;
            BouncingAnimationView.OnFinish += BouncingAnimationView_OnFinish;
        }



        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, ViewMessagingConstants.Landscape);
            _viewModel = BindingContext as GameViewModel;

            if(!_isInitialized)
            {
                 SetupEvents();
                 _viewModel.StartPolling();
                _isInitialized = true;
            }
            _viewModel.OnAppearing();
        }



        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, ViewMessagingConstants.Unspecified);
            _viewModel.OnDisappearing();
        }



        private async void BouncingAnimationView_OnFinish(object sender, EventArgs e)
        {
            await BouncingAnimationView.ScaleTo(0, 500, Easing.BounceIn);
            YourTurnContentView.IsVisible = false;
        }



        private async Task ViewModel_NewMatchSnapshot(MatchSnapshot snapshot)
        {
            if (DeletingCardAnimation is { })
            {
                await DeletingCardAnimation;
                _clickedImageButton.Scale = 1;
                DeletingCardAnimation = null;
            }
        }


        private async void ViewModel_YourTurnEventHandler(object sender, EventArgs e)
        {
            YourTurnContentView.IsVisible = true;
            YourTurnContentView.Opacity = 1;
            BouncingAnimationView.Scale = 0;
            await BouncingAnimationView.ScaleTo(1, 500, Easing.BounceIn);
            BouncingAnimationView.IsPlaying = false;
            BouncingAnimationView.IsPlaying = true;
        }


        private void ViewModel_DroppedCardsUpdated(object sender, EventArgs e)
        {
            var cards = _viewModel.DroppedCards;
            var start = DroppedCardsCollectionView.WidthRequest;
            var height = DroppedCardsCollectionView.Height;
            var width = height * Card.Ratio * 1;
            var spacing = width * 0.3;
            new Animation(val => DroppedCardsCollectionView.WidthRequest = val, start, width * cards.Count + spacing * (cards.Count-1),
                Easing.CubicOut).Commit(this, "droppedScale", 32U, 600U);
        }

        private void ViewModel_PlayerCardsUpdated(object sender, EventArgs e)
        {
            var cards = _viewModel.PlayerCards;
            var start = PlayerCollectionView.Width;
            var height = PlayerCollectionView.Height;
            var width = height * Card.Ratio * 1;


            new Animation(val => PlayerCollectionView.WidthRequest = val, start, width * cards.Count,
                Easing.CubicOut).Commit(this, "playerScale", 32U, 600U);
        }


        private void ImageButton_OnClicked(object sender, EventArgs e)
        {
            _clickedImageButton = sender as ImageButton; 
            DeletingCardAnimation = _clickedImageButton.ScaleTo(0, 500U, Easing.CubicOut);
        }


        protected override bool OnBackButtonPressed()
        {
            _viewModel.Exit();
            return base.OnBackButtonPressed();
        }
    }
}