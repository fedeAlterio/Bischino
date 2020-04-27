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
        private readonly RoomQuery _roomQuery;
        private IList<string> _disconnectedPlayers = new List<string>();
        private CancellationTokenSource _pollingTokenSource;
        private Task _pollingTask;
        private bool _isGameEnded;


        public ObservableCollection<Card> PlayerCards { get; set; } = new ObservableCollection<Card>();
        public ObservableCollection<CardWrapper> DroppedCards { get; set; } = new ObservableCollection<CardWrapper>();
        public ObservableCollection<PrivatePlayerWrapper> PrivatePlayers { get; set; } = new ObservableCollection<PrivatePlayerWrapper>();



        private Player _player;
        public Player Player
        {
            get => _player;
            set => SetProperty(ref _player, value);
        }


        private readonly IList<MatchSnapshot> _matchChronology = new List<MatchSnapshot>();
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



        private bool _isChronologyStarted;
        public bool IsChronologyStarted
        {
            get => _isChronologyStarted;
            set => SetProperty(ref _isChronologyStarted, value);
        }



        private Command _nextSnapshotCommand;
        public Command NextSnapshotCommand
        {
            get => _nextSnapshotCommand;
            set => SetProperty(ref _nextSnapshotCommand, value);
        }
        private async Task NextSnapshot()
        {
            IsPageEnabled = false;
            var nextSnapshot = _matchChronology.Last() == _matchSnapshot
                ? _matchChronology.First()
                : _matchChronology[_matchChronology.IndexOf(_matchSnapshot) + 1];
            await HandleChronologySnapshot(nextSnapshot);
            IsPageEnabled = true;
        }



        private Command _previousSnapshotCommand;
        public Command PreviousSnapshotCommand
        {
            get => _previousSnapshotCommand;
            set => SetProperty(ref _previousSnapshotCommand, value);
        }
        private async Task PreviousSnapshot()
        {
            IsPageEnabled = false;
            var previousSnapshot = _matchChronology.First() == _matchSnapshot
                ? _matchChronology.Last()
                : _matchChronology[_matchChronology.IndexOf(_matchSnapshot) - 1];
            await HandleChronologySnapshot(previousSnapshot);
            IsPageEnabled = true;
        }



        private Command<string> _goToSnapshotCommand;
        public Command<string> GoToSnapshotCommand
        {
            get => _goToSnapshotCommand;
            set => SetProperty(ref _goToSnapshotCommand, value);
        }
        private async void GoToSnapshot(string indexString)
        {
            if(!int.TryParse(indexString, out var index) || index < 0 || index >= _matchChronology.Count)
                Notify(nameof(ChronologySnapshotIndex));
            else
            {
                _matchSnapshot = _matchChronology[index];
                Notify(nameof(ChronologySnapshotIndex));
                await HandleChronologySnapshot(_matchSnapshot);
                IsPageEnabled = true;
            }
        }



        public int ChronologySnapshotIndex => _matchChronology.IndexOf(_matchSnapshot);



        private async void Drop(Card card)
        {
            IsPageEnabled = false;

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

            IsPageEnabled = true;
        }
        private bool CanDrop() => IsDropPhase;



        private Command _audioSettingsCommand;
        public Command AudioSettingsCommand
        {
            get => _audioSettingsCommand;
            set => SetProperty(ref _audioSettingsCommand, value);
        }

        private async void ToAudioSettings()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ShowAudioPopup();
            IsPageEnabled = true;
        }



        private Command _exitCommand;
        public Command ExitCommand
        {
            get => _exitCommand;
            set => SetProperty(ref _exitCommand, value);
        }

        public async Task Exit()
        {
            IsPageEnabled = false;
            await StopPolling();
            await AppController.Navigation.GameNavigation.BackToRoomList(true);
            IsPageEnabled = true;
        }



        public GameViewModel(Room room, RoomManager roomInfo)
        {
            Player = AppController.Navigation.GameNavigation.LoggedPlayer;
            _room = room;
            _roomInfo = roomInfo;
            _roomQuery = new RoomQuery {PlayerName = Player.Name, RoomName = _room.Name};
            DropCommand = new Command<Card>(Drop, _ => CanDrop());
            AudioSettingsCommand = new Command(_ => ToAudioSettings());
            ExitCommand = new Command(async _ => await Exit());
            NextSnapshotCommand = new Command(async _ => await NextSnapshot());
            PreviousSnapshotCommand = new Command(async _ => await PreviousSnapshot());
            GoToSnapshotCommand = new Command<string>(GoToSnapshot);
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

        private async Task<int> GetVersionNumber(CancellationToken token)
        {
            while(true)
                try
                {
                    return await AppController.GameHandler.GetCurrentSnapshotNumber(_roomQuery, token);
                }
                catch (Exception) when (!token.IsCancellationRequested)
                {
                    await Task.Delay(500);
                }
        }



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
            DropCommand.ChangeCanExecute();

            if (matchSnapshot.Winners is { })
            {
                await OnWinnersPhase();
                return;
            }

            HandleTurn(matchSnapshot.PlayerTurn);
            if (matchSnapshot.PlayerTurn.Name == Player.Name)
                if (matchSnapshot.IsEndTurn)
                    ;//await OnEndTurn();
                else if (matchSnapshot.IsPhaseEnded)
                    ;//await OnEndPhase();
        }



        public void StartChronology()
        {
            IsChronologyStarted = true;
            ChronologyStarted?.Invoke(this, EventArgs.Empty);
        }



        private async void ToLastPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToLastPhase(_room.Name, Player.Name,
            MatchSnapshot.Player.LastPhaseViewModel);
            IsPageEnabled = true;
        }

    
        private async void ToBetPhase()
        {
            IsPageEnabled = false;
            await AppController.Navigation.GameNavigation.ToBetPopup(_room.Name, _matchSnapshot);
            IsPageEnabled = true;
        }

        
        public void BetCompleted() => IsBetPhase = false;
        public void PaoloSent() => IsDropPhase = false;
        public void LastPhaseCompleted() => IsLastPhase = false;



        private void UpdatePlayerCards()
        {
            if (_matchSnapshot.Player.StartCardsCount == 1 && _matchSnapshot.Player.Cards.Count == 1 &&
                 _matchSnapshot.Player.DropCardViewModel == null)
                return;

            PlayerCards.Clear();
            foreach (var card in _matchSnapshot.Player.Cards)
                PlayerCards.Add(card);
            PlayerCardsUpdated?.Invoke(this, EventArgs.Empty);
        }


        private async Task UpdateDroppedCards()
        {
            if (!_matchSnapshot.DroppedCards.Any())
                await FadeDroppedCards();

            DroppedCards.Clear();
            if(_matchSnapshot.DroppedCards.Any())
                AppController.AudioManager.PlaySound(SoundEffect.Pop);
            foreach (var card in _matchSnapshot.DroppedCards)
                DroppedCards.Add(new CardWrapper(card));
            DroppedCardsUpdated?.Invoke(this, EventArgs.Empty);
        }


        private async Task FadeDroppedCards()
        {
            if (!DroppedCards.Any())
                return;
            var card = (from c in DroppedCards orderby c.Card.Value select c).LastOrDefault();
            var others = from c in DroppedCards where c != card select c.ScaleTo(0);
            await Task.WhenAll(others);
            await Task.Delay(500);
            if (card != null) 
                await card.ScaleTo(0);
        }


        private async Task<bool> TryNotifyEndPhase()
        {
            try
            {
                await AppController.GameHandler.NextPhase(_roomQuery);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private async Task<bool> TryNotifyEndTurn()
        {
            try
            {
                await AppController.GameHandler.NextTurn(_roomQuery);
                return true;
            }
            catch
            {
                return false;
            }
        }


        private void UpdatePrivatePlayers()
        {
            var players = from player in _matchSnapshot.OtherPlayers select new PrivatePlayerWrapper(player, (uint)_roomInfo.InGameTimeout){IsGameEnded = _isGameEnded};
            PrivatePlayers.Clear();
            foreach (var p in players)
                PrivatePlayers.Add(p);
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


        private async Task HandleChronologySnapshot (MatchSnapshot snapshot)
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
