using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class NameSelectionViewModel : PageViewModel
    {
        private Player _player;
        public Player Player
        {
            get
            {
                NextCommand.ChangeCanExecute();
                return _player;
            }
            set => SetProperty(ref _player, value);
        }


        private Command _nextCommand;
        public Command NextCommand
        {
            get => _nextCommand;
            set => SetProperty(ref _nextCommand, value);
        }


        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }



        public NameSelectionViewModel()
        {
            Player = new Player();
            NextCommand = new Command(_=>Next(), _=>CanGoNext());
        }

        private async void Next()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.NotifyNameSelected(_player);
            IsPageEnabled = true;
        }

        private bool CanGoNext()
        {
            if (string.IsNullOrWhiteSpace(_player.Name))
            {
                ErrorMessage = string.Empty;
                return false;
            }
            if (_player.Name.Any(char.IsWhiteSpace))
            {
                ErrorMessage = "Make sure there are no spaces";
                return false;
            }
            if (_player.Name.Length > 16)
            {
                ErrorMessage = "Make sure the username is at least 16 character long";
                return false;
            }

            ErrorMessage = string.Empty;
            return true;
        }
    }
}
