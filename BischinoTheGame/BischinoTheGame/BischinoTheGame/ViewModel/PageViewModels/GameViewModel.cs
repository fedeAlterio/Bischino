using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BischinoTheGame.Controller;
using BischinoTheGame.Controller.Communication.Exceptions;
using BischinoTheGame.Controller.Communication.Queries;
using BischinoTheGame.Model;
using BischinoTheGame.View.ViewElements;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class GameViewModel : PageViewModel
    {
        public event EventHandler DroppedCardsUpdated;
        public event EventHandler PlayerCardsUpdated;
        public event EventHandler YourTurn;
        public event Func<MatchSnapshot, Task> NewMatchSnapshot;
        public event Func<Task> DropFailed;
        public event EventHandler ChronologyStarted;

        private readonly Room _room;
        private readonly RoomManager _roomInfo;
        private readonly IList<MatchSnapshot> _matchChronology = new List<MatchSnapshot>();
        private readonly RoomQuery _roomQuery;


        private IList<string> _disconnectedPlayers = new List<string>();
        private CancellationTokenSource _pollingTokenSource;
        private Task _pollingTask;
        private bool _isGameEnded;


        // Initialization
        public GameViewModel(Room room, RoomManager roomInfo)
        {
            Player = AppController.Navigation.GameNavigation.LoggedPlayer;
            _room = room;
            _roomInfo = roomInfo;
            _roomQuery = new() { PlayerName = Player.Name, RoomName = _room.Name };
            DropCommand = NewCommand<Card>(Drop, CanDrop);
            AudioSettingsCommand = NewCommand(ToAudioSettings);
            ExitCommand = NewCommand(Exit);
            NextSnapshotCommand = NewCommand(NextSnapshot);
            PreviousSnapshotCommand = NewCommand(PreviousSnapshot);
            GoToSnapshotCommand = NewCommand<string>(GoToSnapshot);
            ToLastPhaseCommand = NewCommand(ToLastPhase);
            ToBetPhaseCommand = NewCommand(ToBetPhase);

            PlayerCards = new (_playerCards);
            DroppedCards = new (_droppedCards);
            PrivatePlayers = new (_privatePlayers);
        }



        // Commands
        public IAsyncCommand<Card> DropCommand { get; }
        public IAsyncCommand AudioSettingsCommand { get; }
        public IAsyncCommand ExitCommand { get; }
        public IAsyncCommand NextSnapshotCommand { get; }
        public IAsyncCommand PreviousSnapshotCommand { get; }
        public IAsyncCommand<string> GoToSnapshotCommand { get; }
        public IAsyncCommand ToLastPhaseCommand { get; }
        public IAsyncCommand ToBetPhaseCommand { get; }


        // Properties        
        private readonly ObservableCollection<Card> _playerCards = new();
        public ReadOnlyObservableCollection<Card> PlayerCards { get; }


        private readonly ObservableCollection<CardWrapper> _droppedCards = new();
        public ReadOnlyObservableCollection<CardWrapper> DroppedCards { get; }


        private readonly ObservableCollection<PrivatePlayerWrapper> _privatePlayers = new();
        public ReadOnlyObservableCollection<PrivatePlayerWrapper> PrivatePlayers { get; }


        public Player Player { get; }
        public int ChronologySnapshotIndex => _matchChronology.IndexOf(_matchSnapshot);


        private MatchSnapshot _matchSnapshot;
        public MatchSnapshot MatchSnapshot
        {
            get => _matchSnapshot;
            set
            {
                if (!_isGameEnded)
                {
                    _matchChronology.Add(value);
                    SetProperty(ref _matchSnapshot, value);
                }
                else
                {
                    SetProperty(ref _matchSnapshot, value);
                    Notify(nameof(ChronologySnapshotIndex));
                }
            }
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
                    ToBetPhaseCommand.Execute(null);
            }
        }


        private bool _isLastPhase;
        public bool IsLastPhase
        {
            get => _isLastPhase;
            set
            {
                if (SetProperty(ref _isLastPhase, value) && value)
                    ToLastPhaseCommand.Execute(null);
            }
        }

        private bool _isChronologyStarted;
        public bool IsChronologyStarted
        {
            get => _isChronologyStarted;
            set => SetProperty(ref _isChronologyStarted, value);
        }




        // Commands Handlers
        private async Task UpdateDroppedCards()
        {
            if (!_matchSnapshot.DroppedCards.Any())
                await FadeDroppedCards();

            _droppedCards.Clear();
            if (_matchSnapshot.DroppedCards.Any())
                AppController.AudioManager.PlaySound(SoundEffect.Pop);
            foreach (var card in _matchSnapshot.DroppedCards)
                _droppedCards.Add(new CardWrapper(card));
            DroppedCardsUpdated?.Invoke(this, EventArgs.Empty);
        }


        private async Task FadeDroppedCards()
        {
            if (!_droppedCards.Any())
                return;
            var card = (from c in _droppedCards orderby c.Card.Value select c).LastOrDefault();
            var others = from c in _droppedCards where c != card select c.ScaleTo(0);
            await Task.WhenAll(others);
            await Task.Delay(500);
            if (card != null)
                await card.ScaleTo(0);
        }



        private async Task NextSnapshot()
        {
            var nextSnapshot = _matchChronology.Last() == _matchSnapshot
                ? _matchChronology.First()
                : _matchChronology[_matchChronology.IndexOf(_matchSnapshot) + 1];
            await HandleChronologySnapshot(nextSnapshot);
        }



        private async Task PreviousSnapshot()
        {
            var previousSnapshot = _matchChronology.First() == _matchSnapshot
                ? _matchChronology.Last()
                : _matchChronology[_matchChronology.IndexOf(_matchSnapshot) - 1];
            await HandleChronologySnapshot(previousSnapshot);
        }


        private async Task GoToSnapshot(string indexString)
        {
            if (!int.TryParse(indexString, out var index) || index < 0 || index >= _matchChronology.Count)
                Notify(nameof(ChronologySnapshotIndex));
            else
            {
                _matchSnapshot = _matchChronology[index];
                Notify(nameof(ChronologySnapshotIndex));
                await HandleChronologySnapshot(_matchSnapshot);
                IsPageEnabled = true;
            }
        }





        private async Task Drop(Card card)
        {
            if (card.IsPaolo)
                await AppController.Navigation.GameNavigation.ToPaoloPopup(_room.Name, Player.Name);
            else
                try
                {
                    var query = new RoomQuery<string> { PlayerName = Player.Name, RoomName = _room.Name, Data = card.Name };
                    await AppController.GameHandler.DropCard(query);
                    IsDropPhase = false;
                }
                catch (ServerException e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, e.Message);
                    await DropFailed.Invoke();
                }
                catch (Exception e)
                {
                    await AppController.Navigation.DisplayAlert(ErrorTitle, "You can not drop this card, check your internet connection");
                    await DropFailed.Invoke();
                }
        }
        private bool CanDrop() => IsDropPhase;




        private async Task ToAudioSettings()
        {
            await AppController.Navigation.GameNavigation.ShowAudioPopup();
        }


        public async Task Exit()
        {
            await StopPolling();
            await AppController.Navigation.GameNavigation.BackToRoomList(true);
        }
        
        private async Task<int> GetVersionNumber(CancellationToken token)
        {
            while (true)
                try
                {
                    return await AppController.GameHandler.GetCurrentSnapshotNumber(_roomQuery, token);
                }
                catch (Exception) when (!token.IsCancellationRequested)
                {
                    await Task.Delay(500);
                }
        }



        public void StartChronology()
        {
            IsChronologyStarted = true;
            ChronologyStarted?.Invoke(this, EventArgs.Empty);
        }



        private async Task ToLastPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToLastPhase(_room.Name, Player.Name,
            MatchSnapshot.Player.LastPhaseViewModel);
            IsPageEnabled = true;
        }


        private async Task ToBetPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToBetPopup(_room.Name, _matchSnapshot);
            IsPageEnabled = true;
        }


        // Helpers
        private async Task HandleSnapshot(MatchSnapshot matchSnapshot)
        {
            if (matchSnapshot is null || MatchSnapshot != null && matchSnapshot.Version <= MatchSnapshot.Version)
                return;

            MatchSnapshot = matchSnapshot;

            await HandleDisconnectedPlayers();
            await NewMatchSnapshot.Invoke(_matchSnapshot);
            await UpdateDroppedCards();
            UpdatePrivatePlayers();
            UpdatePlayerCards();

            IsBetPhase = matchSnapshot.Player.BetViewModel is { };
            IsDropPhase = matchSnapshot.Player.DropCardViewModel is { };
            IsLastPhase = matchSnapshot.Player.LastPhaseViewModel is { };
            DropCommand.RaiseCanExecuteChanged();

            if (matchSnapshot.Winners is { })
            {
                await OnWinnersPhase();
                return;
            }

            HandleTurn(matchSnapshot.PlayerTurn);
        }




        private async Task LongPolling(CancellationToken token)
        {
            var isUpdated = false;
            while (!token.IsCancellationRequested && !_isGameEnded)
            {
                try
                {
                    if (!isUpdated)
                    {
                        await UpdateToLastSnapshot(token);
                        isUpdated = true;
                    }
                    else
                    {
                        var snapshot = await AppController.GameHandler.GetMatchSnapshot(_roomQuery, token);
                        await HandleSnapshot(snapshot);
                    }
                }
                catch (OperationCanceledException e)
                {
                    return;
                }
                catch (Exception)
                {
                    try
                    {
                        await UpdateToLastSnapshot(token);
                    }
                    catch (Exception) when (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        private async Task UpdateToLastSnapshot(CancellationToken token)
        {
            var snapshotUpdated = false;
            var versionNumber = await GetVersionNumber(token);
            if (MatchSnapshot != null && versionNumber <= MatchSnapshot.Version)
                return;

            while (!snapshotUpdated)
                try
                {
                    var snapshot = await AppController.GameHandler.GetMatchSnapshotForced(_roomQuery, token);
                    await HandleSnapshot(snapshot);
                    snapshotUpdated = true;
                }
                catch (Exception) when (!token.IsCancellationRequested)
                {
                    await Task.Delay(500);
                }
        }



        public void StartPolling()
        {
            _pollingTokenSource = new CancellationTokenSource();
            _pollingTask = LongPolling(_pollingTokenSource.Token);
        }

        public async Task StopPolling()
        {
            if (_pollingTokenSource is null)
                return;

            _pollingTokenSource.Cancel();
            await _pollingTask;
        }

        public void BetCompleted() => IsBetPhase = false;
        public void PaoloSent() => IsDropPhase = false;
        public void LastPhaseCompleted() => IsLastPhase = false;



        private void UpdatePlayerCards()
        {
            if (_matchSnapshot.Player.StartCardsCount == 1 && _matchSnapshot.Player.Cards.Count == 1 &&
                 _matchSnapshot.Player.DropCardViewModel == null)
                return;

            _playerCards.Clear();
            foreach (var card in _matchSnapshot.Player.Cards)
                _playerCards.Add(card);
            PlayerCardsUpdated?.Invoke(this, EventArgs.Empty);
        }


        private void UpdatePrivatePlayers()
        {
            var players = from player in _matchSnapshot.OtherPlayers select new PrivatePlayerWrapper(player, (uint)_roomInfo.InGameTimeout) { IsGameEnded = _isGameEnded };
            _privatePlayers.Clear();
            foreach (var p in players)
                _privatePlayers.Add(p);
        }


        private async Task HandleDisconnectedPlayers()
        {
            var names = from p in MatchSnapshot.OtherPlayers where p.IsIdled select p.Name;
            var enumerable = names as string[] ?? names.ToArray();
            if (enumerable.All(name => _disconnectedPlayers.Contains(name)))
                return;

            AppController.AudioManager.PlaySound(SoundEffect.Disconnected);
            _disconnectedPlayers = enumerable.ToList();
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
        }


        private async Task OnWinnersPhase()
        {
            IsPageEnabled = false;

            foreach (var player in PrivatePlayers)
                player.StopClock();

            await Task.Delay(3000);
            AppController.AudioManager.PlaySound(SoundEffect.Win);
            await AppController.Navigation.GameNavigation.ToWinnersPopup(MatchSnapshot);

            _isGameEnded = true;
            IsPageEnabled = true;
        }


        private void HandleTurn(PrivatePlayer playerTurn)
        {
            if (playerTurn.Name == Player.Name)
                if (MatchSnapshot.Player.DropCardViewModel is { })
                    YourTurn?.Invoke(this, EventArgs.Empty);
        }


        private async Task HandleChronologySnapshot(MatchSnapshot snapshot)
        {
            MatchSnapshot = snapshot;
            await UpdateDroppedCards();
            UpdatePrivatePlayers();
            UpdatePlayerCards();
        }


        public async Task OnDisappearing()
        {
            foreach (var p in PrivatePlayers)
                p.OnDisappearing();
            await StopPolling();
        }

        public void OnAppearing()
        {
            foreach (var p in PrivatePlayers)
                p.OnAppearing();
            StartPolling();
        }
    }
}
