using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using Rg.Plugins.Popup.Services;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class GameViewModel : PageViewModel
    {
        public event EventHandler DroppedCardsUpdated;
        public event EventHandler PlayerCardsUpdated;
        public event EventHandler YourTurn;
        public event Func<Task> NewMatchSnapshot;
        private readonly Room _room;
        private readonly RoomQuery _roomQuery;
        private IList<string> _disconnectedPlayers = new List<string>();

        public ObservableCollection<Card> PlayerCards { get; set; } = new ObservableCollection<Card>();
        public ObservableCollection<Card> DroppedCards { get; set; } = new ObservableCollection<Card>();
        public bool IsGameEnded { get; set; }
        private bool CanDrop() => IsDropPhase;


        private Player _player;
        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }

        private MatchSnapshot _matchSnapshot;
        public MatchSnapshot MatchSnapshot
        {
            get => _matchSnapshot;
            set => SetProperty(ref _matchSnapshot, value);
        }


        private bool _isDropPhase;
        public bool IsDropPhase
        {
            get => _isDropPhase;
            set => SetProperty(ref _isDropPhase, value);
        }


        private bool _isBetPhase;
        public bool IsBetPhase
        {
            get => _isBetPhase;
            set
            {
                if (SetProperty(ref _isBetPhase, value) && value)
                    ToBetPhase();
            }
        }


        private bool _isLastPhase;
        public bool IsLastPhase
        {
            get => _isLastPhase;
            set
            {
                if (SetProperty(ref _isLastPhase, value) && value)
                    ToLastPhase();
            }
        }


        private Command<Card> _dropCommand;
        public Command<Card> DropCommand
        {
            get => _dropCommand;
            set => SetProperty(ref _dropCommand, value);
        }


        public GameViewModel(Room room)
        {
            Player = AppController.Navigation.RoomNavigation.LoggedPlayer;
            _room = room;
            _roomQuery = new RoomQuery {PlayerName = Player.Name, RoomName = _room.Name};
            DropCommand = new Command<Card>(Drop, _ => CanDrop());
            AppController.RoomsHandler.MatchSnapshotUpdated += HandleSnapshot;
            AppController.RoomsHandler.SubscribeMatchSnapshotUpdates(_roomQuery);
        }


        private async void ToLastPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ToLastPhase(_room.Name, Player.Name,
                MatchSnapshot.Player.LastPhaseViewModel);
            IsPageEnabled = true;
        }


        private async void ToBetPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ToBetPopup(_room.Name, Player.Name,
                _matchSnapshot.Player.BetViewModel);
            IsPageEnabled = true;
        }

        public void BetCompleted()
        {
            IsBetPhase = false;
        }

        public void PaoloSent()
        {
            IsDropPhase = false;
        }

        public void LastPhaseCompleted()
        {
            IsLastPhase = false;
        }

        private async void Drop(Card card)
        {
            IsPageEnabled = false;

            if (card.IsPaolo)
            {
                await AppController.Navigation.RoomNavigation.ToPaoloPopup(_room.Name, Player.Name);
            }
            else
                try
                {
                    var query = new RoomQuery<string>
                        {PlayerName = Player.Name, RoomName = _room.Name, Data = card.Name};
                    await AppController.RoomsHandler.DropCard(query);
                    IsDropPhase = false;
                }
                catch (ServerException e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
                }
                catch (Exception)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, ErrorDefault);
                }

            IsPageEnabled = true;
        }


        private void UpdatePlayerCards()
        {
            if (_matchSnapshot.Player.Cards.Count == PlayerCards.Count ||
                (_matchSnapshot.Player.StartCardsCount == 1 && _matchSnapshot.Player.Cards.Count == 1 &&
                 _matchSnapshot.Player.DropCardViewModel == null))
                return;

            PlayerCards.Clear();
            foreach (var card in _matchSnapshot.Player.Cards)
                PlayerCards.Add(card);
            PlayerCardsUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateDroppedCards()
        {
            if (_matchSnapshot.DroppedCards.Count == DroppedCards.Count)
                return;

            DroppedCards.Clear();
            foreach (var card in _matchSnapshot.DroppedCards)
                DroppedCards.Add(card);
            DroppedCardsUpdated?.Invoke(this, EventArgs.Empty);
        }


        private async Task OnEndPhase()
        {
            await Task.Delay(3000);
            bool isOk = false;
            while(!isOk)
                try
                {
                    await AppController.RoomsHandler.NextPhase(_roomQuery);
                    isOk = true;
                }
                catch
                {
                    await AppController.Navigation.DisplayAlert("Error",
                        "Impossible to communicate with the server, try again");
                }
        }

        private async Task OnEndTurn()
        {
            await Task.Delay(3000);
            bool isOk = false;
            while (!isOk)
                try
                {
                    await AppController.RoomsHandler.NextTurn(_roomQuery);
                    isOk = true;
                }
                catch
                {
                    await AppController.Navigation.DisplayAlert("Error",
                        "Impossible to communicate with the server, try again");
                }
        }

        private async void HandleSnapshot(object sender, MatchSnapshot matchSnapshot)
        {
            MatchSnapshot = matchSnapshot;
            if (NewMatchSnapshot == null || IsGameEnded)
                return;


            await HandleDisconnectedPlayers();
            await NewMatchSnapshot?.Invoke();
            UpdatePlayerCards();
            UpdateDroppedCards();

            IsBetPhase = matchSnapshot.Player.BetViewModel is {};
            IsDropPhase = matchSnapshot.Player.DropCardViewModel is { };
            IsLastPhase = matchSnapshot.Player.LastPhaseViewModel is {};
            DropCommand.ChangeCanExecute();

            HandleTurn(matchSnapshot.PlayerTurn);

            if (matchSnapshot.PlayerTurn.Name == Player.Name)
                if (matchSnapshot.IsEndTurn)
                    await OnEndTurn();
                else if (matchSnapshot.IsPhaseEnded)
                    await OnEndPhase();

            if (matchSnapshot.Winners is { })
                await OnWinnersPhase();
        }

        private async Task HandleDisconnectedPlayers()
        {
            var names = from p in MatchSnapshot.OtherPlayers where p.IsIdled select p.Name;
            var enumerable = names as string[] ?? names.ToArray();
            if (enumerable.All(name => _disconnectedPlayers.Contains(name)))
                return;

            _disconnectedPlayers = enumerable.ToList();
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
        }

        private async Task OnWinnersPhase()
        {
            if (IsGameEnded)
                return;

            IsPageEnabled = false;
            IsGameEnded = true;
            UnsubscribeFromEvents();
            await Task.Delay(3000);
            await AppController.Navigation.RoomNavigation.ToWinnersPopup(MatchSnapshot);
            IsPageEnabled = true;
        }


        private void HandleTurn(PrivatePlayer playerTurn)
        {
            if (playerTurn.Name == Player.Name)
                if (MatchSnapshot.Player.DropCardViewModel is { })
                    YourTurn?.Invoke(this, EventArgs.Empty);
        }


        private void UnsubscribeFromEvents()
        {
            AppController.RoomsHandler.UnsubscribeMatchSnapshotUpdates();
            AppController.RoomsHandler.MatchSnapshotUpdated -= HandleSnapshot;
        }
    }
}
