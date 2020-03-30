using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Navigation.RoomNavigation;

namespace Rooms.Controller.Navigation
{
    public interface IAppNavigation
    {
        Task DisplayAlert(string title, string message);
        IRoomNavigation RoomNavigation { get; }

        Task PopPopupAsync();
    }
}
