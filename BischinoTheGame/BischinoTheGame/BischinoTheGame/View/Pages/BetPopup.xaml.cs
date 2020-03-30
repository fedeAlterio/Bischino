using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BischinoTheGame.View.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BetPopup : PopupPage
    {
        public BetPopup()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true; // Disable back button
        }
    }
}