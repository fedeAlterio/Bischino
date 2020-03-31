using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
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


        public ObservableCollection<Card> PlayerCards { get; set; } = new ObservableCollection<Card>();
        public ObservableCollection<Card> DroppedCards { get; set; } = new ObservableCollection<Card>();

        public GameViewModel(Room room)
        {
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
                catch (Exception e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
                }

            IsPageEnabled = true;
        }


        private void UpdatePlayerCards()
        {
            if (_matchSnapshot.Player.Cards.Count == PlayerCards.Count ||
                (_matchSnapshot.Player.StartCardsCount == 1 && _matchSnapshot.Player.DropCardViewModel == null))
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
            await AppController.RoomsHandler.NextPhase(_roomQuery);
        }

        private async Task OnEndTurn()
        {
            await Task.Delay(3000);
            await AppController.RoomsHandler.NextTurn(_roomQuery);
        }

        private async void HandleSnapshot(object sender, MatchSnapshot matchSnapshot)
        {
            MatchSnapshot = matchSnapshot;
            if (NewMatchSnapshot == null)
                return;


            await NewMatchSnapshot?.Invoke();

            if (matchSnapshot.DisconnectedPlayer is { })
            {
                await OnPlayerDisconnected(matchSnapshot.DisconnectedPlayer);
                return;
            }

            UpdatePlayerCards();
            UpdateDroppedCards();

            IsBetPhase = matchSnapshot.Player.BetViewModel is {};
            IsDropPhase = matchSnapshot.Player.DropCardViewModel is { };
            IsLastPhase = matchSnapshot.Player.LastPhaseViewModel is {};
            DropCommand.ChangeCanExecute();

            HandleTurn(matchSnapshot.PlayerTurn);

            if (matchSnapshot.Host == Player.Name)
                if (matchSnapshot.IsEndTurn)
                    await OnEndTurn();
                else if (matchSnapshot.IsPhaseEnded)
                    await OnEndPhase();

            if (matchSnapshot.Winners is { })
                await OnWinnersPhase();
        }

        private async Task OnWinnersPhase()
        {
            IsPageEnabled = false;
            UnsubscribeFromEvents();
            await Task.Delay(3000);
            await AppController.Navigation.RoomNavigation.ToWinnersPopup(MatchSnapshot);
            IsPageEnabled = true;
        }

        private async Task OnPlayerDisconnected(string playerName)
        {
            IsPageEnabled = false;
            UnsubscribeFromEvents();
            await AppController.Navigation.DisplayAlert("Timeout",
                $"The player {playerName} is idled, going back to room list");
            await AppController.Navigation.RoomNavigation.BackToRoomList();
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

        private Player Player { get; } = AppController.Navigation.RoomNavigation.LoggedPlayer;

        private bool CanDrop() => IsDropPhase;
    }
}
