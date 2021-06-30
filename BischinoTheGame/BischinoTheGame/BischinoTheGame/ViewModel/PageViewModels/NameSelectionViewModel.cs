using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class NameSelectionViewModel : PageViewModel
    {
        // Initialization
        public NameSelectionViewModel()
        {
            Player = new Player();
            NextCommand = NewCommand(Next, CanGoNext);
            Player.PropertyChanged += (_, __) => NextCommand.RaiseCanExecuteChanged();
        }

        // Commands
        public IAsyncCommand NextCommand { get; }


        // Properties
        public Player Player { get; }


        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }



       
        // Commands Handlers
        private async Task Next()
        {
            await AppController.Navigation.GameNavigation.NotifyNameSelected(Player);
        }

        private bool CanGoNext()
        {
            if (string.IsNullOrWhiteSpace(Player.Name))
            {
                ErrorMessage = string.Empty;
                return false;
            }
            if (Player.Name.Any(char.IsWhiteSpace))
            {
                ErrorMessage = "Make sure there are no spaces";
                return false;
            }
            if (Player.Name.Length > 16)
            {
                ErrorMessage = "Make sure the username is at least 16 character long";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}
