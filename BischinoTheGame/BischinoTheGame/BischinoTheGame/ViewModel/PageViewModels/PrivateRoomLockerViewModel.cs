using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class PrivateRoomLockerViewModel : PageViewModel
    {
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



        public Command OneCommand => new Command(_ => Pressed(1));
        public Command TwoCommand => new Command(_ => Pressed(2));
        public Command ThreeCommand => new Command(_ => Pressed(3));
        public Command FourCommand => new Command(_ => Pressed(4));
        public Command FiveCommand => new Command(_ => Pressed(5));
        public Command SixCommand => new Command(_ => Pressed(6));
        public Command SevenCommand => new Command(_ => Pressed(7));
        public Command EightCommand => new Command(_ => Pressed(8));
        public Command NineCommand => new Command(_ => Pressed(9));
        public Command ZeroCommand => new Command(_ => Pressed(0));


        public Command DeleteCommand => new Command(_ => Code = string.Empty);
        public Command GoCommand => new Command(_ => Join());

        private async void Join()
        {
            IsPageEnabled = false;
            try
            {
                RoomQuery.RoomNumber = Convert.ToInt32(_code);
                var room = await AppController.GameHandler.JoinPrivate(RoomQuery);
                await AppController.Navigation.GameNavigation.NotifyRoomJoined(room);
            }
            catch (ServerException e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
            catch
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, "Check your internet connection, or try to refresh the page");
            }
            IsPageEnabled = true;
        }

        private void Pressed(int number)
        {
            if (_code.Length >= 5)
                return;
            Code += number;
        }
    }
}
