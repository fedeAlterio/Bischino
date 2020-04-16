using System;
using System.Collections.Generic;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel.PageViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bischino.UnitTest
{
    [TestClass]
    public class GameViewModelTest
    {
        private Room NewRoom() => new Room
        {
            Host = "Host",
            MaxPlayers = 6,
            MinPlayers = 2,
            Name = "RoomName",
            PendingPlayers = new List<string>
            {
                "Jhon",
                "Jhosh",
                "Mark"
            }
        };



        private RoomManager NewRoomManager() => new RoomManager()
        {
            InGameTimeout = 70 * 1000,
            IsGameStarted = true,
            StartTime = DateTime.Now
        };



        [TestMethod]
        public void TestMethod1()
        {

        }
    }
}
