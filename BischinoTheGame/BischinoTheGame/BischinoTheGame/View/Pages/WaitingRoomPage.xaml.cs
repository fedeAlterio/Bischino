using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.ViewModel;
using Lottie.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BischinoTheGame.View.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaitingRoomPage : ContentPage
    {
        public WaitingRoomPage()
        {
            InitializeComponent();
        }

        private async void StartButton_OnClicked(object sender, EventArgs e)
        {
            await AnimatedPlayButton.ScaleTo(0.5, 150, Easing.Linear);
            await AnimatedPlayButton.ScaleTo(1, 150, Easing.Linear);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await UnJoin();
        }

        protected override bool OnBackButtonPressed()
        {
            UnJoin();
            return true;
        }

        private async Task UnJoin()
        {
            var vm = BindingContext as WaitingRoomViewModel;
            if (!vm.IsMatchStarted)
                await vm.UnJoin();
        }
    }
}