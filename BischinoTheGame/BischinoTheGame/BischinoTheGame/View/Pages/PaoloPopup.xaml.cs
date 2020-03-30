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
    public partial class PaoloPopup : PopupPage
    {
        public PaoloPopup()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }
    }
}