using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.View.Pages;
using BischinoTheGame.ViewModel;
using BischinoTheGame.ViewModel.PageViewModels;
using Rg.Plugins.Popup.Services;
using Rooms.Controller;
using Rooms.Controller.Navigation;

namespace BischinoTheGame.Navigation.RoomNavigation
{
    public class RoomNavigation : PageNavigationBase, IRoomNavigation
    {
        private GameViewModel _gameViewModel;

        public Player LoggedPlayer { get; private set; }
        public async Task ToRoomSelectionPage()
        {
            var vm = new NameSelectionViewModel();
            var page = new NameSelectionPage {BindingContext = vm};
            await Navigation.PushAsync(page);
        }

        public async Task NotifyNameSelected(Player player)
        {
            LoggedPlayer = player;
            await ToRoomList();
        }

        private async Task ToRoomList()
        {
            var vm = new RoomsListViewModel();
            var page = new RoomsListPage { BindingContext = vm };
            await Navigation.PushAsync(page);
            Navigation.RemovePage(Navigation.NavigationStack.First());
        }
        public async Task ShowRoomCreationPopup()
        {
            var vm = new RoomCreationViewModel();
            var page = new RoomCreationPopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(page);
        }

        public async Task NotifyRoomCreated(Room room)
        {
            await PopupNavigation.Instance.PopAsync();
            await NotifyRoomJoined(room);
        }

        public async Task NotifyRoomJoined(Room room)
        {
            var vm = new WaitingRoomViewModel(room);
            var page = new WaitingRoomPage {BindingContext = vm};
            await Navigation.PushAsync(page);
            Navigation.RemovePage(Navigation.NavigationStack.First());
        }

        public async Task NotifyMatchStarted(Room room)
        {
            _gameViewModel = new GameViewModel(room);
            var page = new GamePage {BindingContext = _gameViewModel};
            await Navigation.PushAsync(page);
            Navigation.RemovePage(Navigation.NavigationStack.First());
        }

        public async Task ToPaoloPopup(string roomName, string playerName)
        {
            var vm = new PaoloPopupViewModel(roomName, playerName);
            var page = new PaoloPopup {BindingContext = vm};

            await PopupNavigation.Instance.PushAsync(page);
        }

        public async Task NotifyPaoloSent()
        {
            await PopupNavigation.Instance.PopAsync();
            _gameViewModel.PaoloSent();
        }

        public async Task ToBetPopup(string roomName, string playerName, BetViewModel betVm)
        {
            var vm = new BetPopupViewModel(roomName, playerName, betVm);
            var popup = new BetPopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(popup);
        }

        public async Task NotifyBetCompleted()
        {
            await PopupNavigation.Instance.PopAsync();
            _gameViewModel.BetCompleted();
        }

        public async Task ToLastPhase(string roomName, string playerName, LastPhaseViewModel lastPhaseVM)
        {
            var vm = new LastPhasePopupViewModel(roomName, playerName, lastPhaseVM);
            var page = new LastPhasePopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(page);
        }

        public async Task NotifyLastPhaseCompleted()
        {
            await PopupNavigation.Instance.PopAsync();
            _gameViewModel.LastPhaseCompleted();
        }

        public async Task ToFilterPopup(RoomSearchQuery query)
        {
            var vm = new FilterPopupViewModel(query);   
            var popup = new FilterPopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(popup);
        }

        public async Task BackToRoomList()
        {
            if(PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
            await ToRoomList();
        }

        public async Task ToWinnersPopup(MatchSnapshot matchSnapshot)
        {
            var vm = new WinnersPopupViewModel(matchSnapshot);
            var popup = new WinnersPopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(popup);
        }
    }
}
