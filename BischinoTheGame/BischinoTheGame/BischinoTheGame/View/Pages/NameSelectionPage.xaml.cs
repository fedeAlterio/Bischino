using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace BischinoTheGame
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NameSelectionPage : PopupPage
    {
        public NameSelectionPage()
        {
            InitializeComponent();
            CloseWhenBackgroundIsClicked = false;
        }
    }
}
