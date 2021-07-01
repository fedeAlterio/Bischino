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
            ErrorMessage = this switch
            {
                _ when string.IsNullOrWhiteSpace(Player.Name) => string.Empty,
                _ when Player.Name.Any(char.IsWhiteSpace) => "Make sure there are no spaces",
                _ when Player.Name.Length > 16 => "Make sure the username is at least 16 character long",
                _ => null
            };
            return ErrorMessage is null;
        }
    }
}
