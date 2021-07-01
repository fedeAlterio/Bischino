using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class PrivateRoomLockerViewModel : PageViewModel
    {
        // Initialization
        public PrivateRoomLockerViewModel()
        {
            DeleteCommand = NewCommand(() => Code = string.Empty);
            GoCommand = NewCommand(Join);

            // Keyboard commands
            int i = 0;
            IAsyncCommand NextKeyCommand()
            {
                var num = i+++1;
                return NewCommand(() => Pressed(num));
            }
            OneCommand = NextKeyCommand();
            TwoCommand = NextKeyCommand();
            ThreeCommand = NextKeyCommand();
            FourCommand = NextKeyCommand();
            FiveCommand = NextKeyCommand();
            SixCommand = NextKeyCommand();
            SevenCommand = NextKeyCommand();
            EightCommand = NextKeyCommand();
            NineCommand = NextKeyCommand();
        }


        // Commands
        public IAsyncCommand DeleteCommand { get; }
        public IAsyncCommand GoCommand { get; }

        public IAsyncCommand OneCommand { get; }
        public IAsyncCommand TwoCommand { get; }
        public IAsyncCommand ThreeCommand { get; }
        public IAsyncCommand FourCommand { get; }
        public IAsyncCommand FiveCommand { get; }
        public IAsyncCommand SixCommand { get; }
        public IAsyncCommand SevenCommand { get; }
        public IAsyncCommand EightCommand { get; }
        public IAsyncCommand NineCommand { get; }
        public IAsyncCommand ZeroCommand { get; }


        // Properties
        private RoomQuery RoomQuery { get; } = new RoomQuery
        {
            PlayerName = AppController.Navigation.GameNavigation.LoggedPlayer.Name
        };

        private string _code = "";
        public string Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }



        // Commands Handlers
        private async Task Join()
        {
            RoomQuery.RoomNumber = Convert.ToInt32(_code);
            var room = await AppController.GameHandler.JoinPrivate(RoomQuery);
            await AppController.Navigation.GameNavigation.NotifyRoomJoined(room);
        }

        private void Pressed(int number)
        {
            if (_code.Length >= 5)
                return;
            Code += number;
        }
    }
}
