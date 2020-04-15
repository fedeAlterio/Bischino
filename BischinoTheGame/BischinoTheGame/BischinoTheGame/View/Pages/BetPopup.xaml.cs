using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.ViewModel.PageViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BischinoTheGame.View.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetPopup : PopupPage
    {
        private const double _fiveBetsWidth = 420;
        private BetPopupViewModel _viewModel;
        private bool _disappearing;
        private Task _animateInfoTask;


        public BetPopup()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!(BindingContext is BetPopupViewModel vm))
                throw new Exception($"Binding context should be of type {nameof(BetInfoPopupViewModel)}");

            _viewModel = vm;

            _viewModel.BetViewModel = _viewModel.BetViewModel;
            BetCollectionView.WidthRequest = _fiveBetsWidth / 6 * _viewModel.BetViewModel.PossibleBets.Count;
            if (_viewModel.IsInfoVisible)
                _animateInfoTask = AnimateInfo();
        }


        private async Task AnimateInfo()
        {
            _disappearing = false;
            while (!_disappearing)
            {
                await InfoButton.ScaleTo(1.2);
                await InfoButton.ScaleTo(1);
            }
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            _disappearing = true;
            if (_animateInfoTask is null)
                return;

            await _animateInfoTask;
            _animateInfoTask = null;
        }


        protected override bool OnBackButtonPressed()
        {
            return true; // Disable back button
        }
    }
}