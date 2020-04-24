using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;

namespace BischinoTheGame.Navigation.RoomNavigation
{
    public interface IGameNavigation
    {
        Player LoggedPlayer { get; }
        Task ToNameSelection();
        Task NotifyNameSelected(Player player);
        Task ShowRoomCreationPopup();
        Task NotifyRoomCreated(Room room);
        Task NotifyRoomJoined(Room room);
        Task NotifyMatchStarted(Room room, RoomManager roomInfo);
        Task ToPaoloPopup(string roomName, string playerName);
        Task NotifyPaoloSent();
        Task ToBetPopup(string roomName, MatchSnapshot snapshot);
        Task NotifyBetCompleted();
        Task ToLastPhase(string roomName, string playerName, LastPhaseViewModel lastPhaseVM);
        Task NotifyLastPhaseCompleted();
        Task ToFilterPopup(RoomSearchQuery query);
        Task BackToRoomList(bool bannerOn = false);
        Task ToWinnersPopup(MatchSnapshot matchSnapshot);
        Task ToDeckSelection();
        Task NotifyDeckChosen();
        Task ShowAudioPopup();
        Task LaunchTutorial();
        Task ToBetInfoPopup(int missingNumber);
        Task ToCredits();
        Task StartChronology();
        Task ToPrivateRoomLocker();
    }
}
