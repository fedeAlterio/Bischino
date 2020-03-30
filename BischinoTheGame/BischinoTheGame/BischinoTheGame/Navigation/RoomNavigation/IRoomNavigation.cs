﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;

namespace BischinoTheGame.Navigation.RoomNavigation
{
    public interface IRoomNavigation
    {
        Player LoggedPlayer { get; }
        Task ToRoomSelectionPage();
        Task NotifyNameSelected(Player player);
        Task ShowRoomCreationPopup();
        Task NotifyRoomCreated(Room room);
        Task NotifyRoomJoined(Room room, bool isHost);
        Task NotifyMatchStarted(Room room);
        Task ToPaoloPopup(string roomName, string playerName);
        Task NotifyPaoloSent();
        Task ToBetPopup(string roomName, string playerName, BetViewModel vm);
        Task NotifyBetCompleted();
        Task ToLastPhase(string roomName, string playerName, LastPhaseViewModel lastPhaseVM);
        Task NotifyLastPhaseCompleted();
        Task ToFilterPopup(RoomSearchQuery query);
    }
}