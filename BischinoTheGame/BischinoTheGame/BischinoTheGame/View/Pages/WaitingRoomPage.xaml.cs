using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}