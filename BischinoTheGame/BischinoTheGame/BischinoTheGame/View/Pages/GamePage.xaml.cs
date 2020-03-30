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
        public const double PlayerCardWidth = 330 / 5.0;
        private GameViewModel _viewModel;
        private ImageButton _clickedImageButton;
        public Task<bool> DeletingCardAnimation { get; set; }
        public GamePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(this, ViewMessagingConstants.Landscape);
            _viewModel = BindingContext as GameViewModel;
            _viewModel.PlayerCardsUpdated += _viewModel_PlayerCardsUpdated;
            _viewModel.DroppedCardsUpdated += _viewModel_DroppedCardsUpdated;
            _viewModel.YourTurn += _viewModel_YourTurnEventHandler;
            _viewModel.StartPolling += _viewModel_StartPolling;
            _viewModel.NewMatchSnapshot += _viewModel_NewMatchSnapshot;
        }

        private async Task _viewModel_NewMatchSnapshot()
        {
            if (DeletingCardAnimation is { })
            {
                await DeletingCardAnimation;
                _clickedImageButton.Scale = 1;
                DeletingCardAnimation = null;
            }
        }

        private void _viewModel_StartPolling(object sender, EventArgs e)
        {
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send(this, ViewMessagingConstants.Unspecified);
        }

        private async void _viewModel_YourTurnEventHandler(object sender, EventArgs e)
        {
            YourTurnContentView.Opacity = 0;
            YourTurnContentView.IsVisible = true;
            await YourTurnContentView.FadeTo(0.8, 1000, Easing.Linear);
            await YourTurnContentView.FadeTo(0, 500, Easing.CubicOut);
            YourTurnContentView.IsVisible = false;
        }

        private void _viewModel_DroppedCardsUpdated(object sender, EventArgs e)
        {
            var Cards = _viewModel.DroppedCards;
            var start = DroppedCardsCollectionView.WidthRequest;
            var height = DroppedCardsCollectionView.Height;
            var width = height * Card.ratio + 4;
            var spacing = width * 0.4;
            new Animation(val => DroppedCardsCollectionView.WidthRequest = val, start, width * Cards.Count + spacing * (Cards.Count-1),
                Easing.CubicOut).Commit(this, "droppedScale", 32U, 600U);
        }

        private void _viewModel_PlayerCardsUpdated(object sender, EventArgs e)
        {
            var Cards = _viewModel.PlayerCards;
            var start = PlayerCollectionView.WidthRequest;
            new Animation(val => PlayerCollectionView.WidthRequest = val, start, PlayerCardWidth * Cards.Count,
                Easing.CubicOut).Commit(this, "playerScale", 32U, 600U);
        }


        private void ImageButton_OnClicked(object sender, EventArgs e)
        {
            _clickedImageButton = sender as ImageButton; 
            DeletingCardAnimation = _clickedImageButton.ScaleTo(0, 500U, Easing.CubicOut);
        }
    }
}