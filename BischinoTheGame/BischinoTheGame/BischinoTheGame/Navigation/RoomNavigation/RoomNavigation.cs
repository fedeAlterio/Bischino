using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.Model.Settings;
using BischinoTheGame.View.Pages;
using BischinoTheGame.ViewModel;
using BischinoTheGame.ViewModel.PageViewModels;
using Rg.Plugins.Popup.Services;
using Rooms.Controller;
using Rooms.Controller.Navigation;
using Xamarin.Forms;

namespace BischinoTheGame.Navigation.RoomNavigation
{
    public class RoomNavigation : PageNavigationBase, IRoomNavigation
    {
        private GameViewModel _gameViewModel;
        private RulesViewModel _rulesVM;
        private CoreTabbedPage _page;
        private bool _deckChanged = false;
        private RoomsListViewModel _roomListVM;

        public Player LoggedPlayer { get; private set; }
        public async Task ToNameSelection()
        {
            var vm = new NameSelectionViewModel();
            var page = new NameSelectionPage {BindingContext = vm};
            await PushCorePage();
            await PopupNavigation.Instance.PushAsync(page);
        }

        public async Task NotifyNameSelected(Player player)
        {
            LoggedPlayer = player; 
            await PopupNavigation.Instance.PopAsync();
            await GetRoomListPage().StartAnimation();
            _roomListVM.AsyncInitialization();
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

        public async Task NotifyMatchStarted(Room room, RoomManager roomInfo)
        {
            _gameViewModel = new GameViewModel(room, roomInfo);
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
            await PushCorePage();
            _roomListVM.AsyncInitialization();
            Navigation.RemovePage(Navigation.NavigationStack.First());
        }

        public async Task ToWinnersPopup(MatchSnapshot matchSnapshot)
        {
            var vm = new WinnersPopupViewModel(matchSnapshot);
            var popup = new WinnersPopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(popup);
        }

        public async Task ToDeckSelection()
        {
            var vm = new DeckSelectionVewModel();
            var page = new DeckSelectionPage {BindingContext = vm};
            await Navigation.PushModalAsync(page);
        }

        public async Task NotifyDeckChosen()
        {
            await Navigation.PopModalAsync();
        }

        public async Task ShowAudioPopup()
        {
            var vm = new AudioPopupViewModel();
            var page = new AudioPopup {BindingContext = vm};
            await PopupNavigation.Instance.PushAsync(page);
        }

        private async Task PushCorePage()
        {
            AppController.Settings.DeckChanged += (_, __) => _deckChanged = true;
            _roomListVM = new RoomsListViewModel();
            var settingsVM = new SettingsViewModel();
            _rulesVM = new RulesViewModel();
            var vm = new CoreTabbedViewModel(_roomListVM, settingsVM, _rulesVM); 
            _page = new CoreTabbedPage {BindingContext = vm};
            _page.CurrentPageChanged += CoreTabbedPage_CurrentPageChanged;
            await Navigation.PushAsync(_page);
        }

        private void CoreTabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {
            if (_page.CurrentPage.BindingContext == _rulesVM)
                if(_deckChanged)
                {
                    _rulesVM.LoadDecks();
                    _deckChanged = false;
                }
        }

        private RoomsListPage GetRoomListPage()
        {
            var query = from page in _page.Children where page is RoomsListPage select (RoomsListPage)page;
            return query.FirstOrDefault();
        }
    }
}
