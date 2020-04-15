using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel;
using Plugin.SimpleAudioPlayer;
using Xamarin.Forms;

namespace BischinoTheGame.View.ViewElements
{
    public class PrivatePlayerWrapper : ViewModelBase
    {
        private bool _isAnimating;
        private readonly uint _periodTimeout;
        private static ISimpleAudioPlayer _audio;
        private bool _started;
        private bool _isBackground;
        private bool _stopped;
        private int _clockTime = 15 * 1000;

        public Color PlayingColor { get; set; } = Color.Transparent; //Color.FromHex("#212121");
        public Color IdledColor { get; set; } = Color.DarkRed;
        public Color TurnColor { get; set; } = Color.LimeGreen;//Color.FromHex("#204051");
        public Color HashLostColor { get; set; } = Color.DarkRed; // Color.FromHex("#121212");


        private PrivatePlayer _player;
        public PrivatePlayer Player
        {
            get => _player;
            private set
            {
                SetProperty(ref _player, value);
                if (value != null && value.IsTurn)
                    OnPlayerTurn();
            }
        }


        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get
            {
                if (_isAnimating)
                    return _backgroundColor;

                if (_player.IsTurn)
                    return TurnColor;

                if (_player.IsIdled)
                    return IdledColor;

                if (_player.HasLost)
                    return HashLostColor;

                return PlayingColor;
            }
            private set
            {
                SetProperty(ref _backgroundColor, value);
                if (_backgroundColor == IdledColor)
                    _isAnimating = false;
            }
        }


        private double _yScale;
        public double YScale
        {
            get => _yScale;
            set
            {
                SetProperty(ref _yScale, value);
                HandleClock(value);
            }
        }


        private double SecToPercentage(int ms) => ms / (double) _periodTimeout;
        private void MuteClock()
        {
            _audio?.Stop();
            _started = false;
        }
        private void HandleClock(double timerLeft)
        {
            if (_stopped)
                return;

            if (_isBackground || timerLeft < 0.001)
                MuteClock();
            else if (timerLeft < SecToPercentage(_clockTime) && !_started)
                StartClock();
        }

        public PrivatePlayerWrapper(PrivatePlayer player, uint periodTimeout)
        {
            _periodTimeout = periodTimeout;
            Player = player;
            YScale = 1;
            MuteClock();
        }


        public void OnDisappearing() => _isBackground = true;
        public void OnAppearing() => _isBackground = false;


        private void OnPlayerTurn()
        {
            double r = TurnColor.R, g = TurnColor.G, b = TurnColor.B;
            new Animation(val => BackgroundColor = new Color(r = val, g, b), r, IdledColor.R,
               Easing.Linear).Commit(Application.Current.MainPage, "RAnimation", 32U, _periodTimeout);
           
            new Animation(val => BackgroundColor = new Color(r, g = val, b), g, IdledColor.G,
                Easing.Linear).Commit(Application.Current.MainPage, "GAnimation", 32U, _periodTimeout);

            new Animation(val => BackgroundColor = new Color(r, g, b = val), b, IdledColor.B,
                Easing.Linear).Commit(Application.Current.MainPage, "BAnimation", 32U, _periodTimeout);

            new Animation(val => YScale = val, 1, 0, Easing.Linear)
                .Commit(Application.Current.MainPage, "ScaleAnimation", 32U, _periodTimeout);
            
            _isAnimating = true;
        }


        private void StartClock()
        {
            if(_audio is null)
            {
                _audio = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                _audio.Load(AudioManager.GetAudioStream("clock.mp3"));
            }
            _audio.Play();
            _started = true;
        }


        public void StopClock()
        {
            _stopped = true;
            MuteClock();
        }
    }
}
