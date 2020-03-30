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
        public event EventHandler StartPolling;
        public event Func<Task> NewMatchSnapshot;

        private readonly Room _room;
        private RoomQuery _roomQuery;

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
                if(SetProperty(ref _isLastPhase, value) && value)
                    ToLastPase();
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
            DropCommand = new Command<Card>(Drop, _=> CanDrop());
            AsyncInitialization();
            AppController.RoomsHandler.MatchSnapshotUpdated += HandleSnapshot;
            Polling();
        }

        private async void AsyncInitialization()
        {
            try
            {
                AppController.RoomsHandler.SubscribeMatchSnapshotUpdates(_roomQuery);
            }
            catch (Exception e)
            {
                await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
            }
        }


        private async void ToLastPase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ToLastPhase(_room.Name, Player.Name, MatchSnapshot.Player.LastPhaseViewModel);
            IsPageEnabled = true;
        }


        private async void ToBetPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.RoomNavigation.ToBetPopup(_room.Name, Player.Name, _matchSnapshot.Player.BetViewModel);
            IsPageEnabled = true;
        }

        public void BetCompleted()
        {
            IsBetPhase = false;
            Polling();
        }

        public void PaoloSent()
        {
            IsDropPhase = false;
            Polling();
        }
        public void LastPhaseCompleted()
        {
            IsLastPhase = false;
            Polling();
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
                    var query = new RoomQuery<string> { PlayerName = Player.Name, RoomName = _room.Name, Data = card.Name};
                    await AppController.RoomsHandler.DropCard(query);
                    IsDropPhase = false;
                    Polling();
                }
                catch (Exception e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
                }

            IsPageEnabled = true;
        }


        private async void Polling()
        {
            /*
            while (PollingEnabled)
            {
                try
                {
                    var snapshot = await AppController.RoomsHandler.GetMatchSnapshot(_roomQuery);
                    await HandleSnapshot(snapshot);
                    await Task.Delay(500);
                }
                catch { }
            }
            */
        }


        private async void HandleSnapshot(object sender, MatchSnapshot matchSnapshot)
        {
            MatchSnapshot = matchSnapshot;
            if(NewMatchSnapshot is { })
                await NewMatchSnapshot?.Invoke();

            if (_matchSnapshot.Player.Cards.Count != PlayerCards.Count && (_matchSnapshot.Player.Cards.Count != 1 || _matchSnapshot.Player.DropCardViewModel is {}))
            {
                PlayerCards.Clear();
                foreach (var card in _matchSnapshot.Player.Cards)
                    PlayerCards.Add(card);
                PlayerCardsUpdated?.Invoke(this, EventArgs.Empty);
            }

            if (_matchSnapshot.DroppedCards.Count != DroppedCards.Count)
            {
                DroppedCards.Clear();
                foreach (var card in _matchSnapshot.DroppedCards)
                    DroppedCards.Add(card);
                DroppedCardsUpdated?.Invoke(this, EventArgs.Empty);
            }
            IsBetPhase = matchSnapshot.Player.BetViewModel is {};
            IsDropPhase = matchSnapshot.Player.DropCardViewModel is { };
            IsLastPhase = matchSnapshot.Player.LastPhaseViewModel is {};
            DropCommand.ChangeCanExecute();

            if(matchSnapshot.Host == Player.Name)
                if (matchSnapshot.IsEndTurn)
                {
                    await Task.Delay(3000);
                    await AppController.RoomsHandler.NextTurn(_roomQuery);
                }
                else if (matchSnapshot.IsPhaseEnded)
                {
                    await Task.Delay(3000);
                    await AppController.RoomsHandler.NextPhase(_roomQuery);
                }

            HandleTurn(matchSnapshot.PlayerTurn);
        }

        private void HandleTurn(PrivatePlayer playerTurn)
        {
            if (playerTurn.Name == Player.Name)
                if(MatchSnapshot.Player.DropCardViewModel is { })
                    YourTurn?.Invoke(this, EventArgs.Empty);
        }


        private Player Player { get; } = AppController.Navigation.RoomNavigation.LoggedPlayer;

        private bool PollingEnabled => !IsBetPhase && !IsDropPhase && !IsLastPhase;
        private bool CanDrop() => IsDropPhase;
    }
}
