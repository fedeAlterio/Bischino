using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel;
using Xamarin.Forms;

namespace BischinoTheGame.View.ViewElements
{
    public class PrivatePlayerWrapper : ViewModelBase
    {
        private bool _isAnimating;
        private uint _periodTimeout;
            
        public Color PlayingColor { get; set; } = Color.FromHex("#212121");
        public Color IdledColor { get; set; } = Color.DarkRed;
        public Color TurnColor { get; set; } = Color.FromHex("#204051");
        public Color HashLostColor { get; set; } = Color.FromHex("#121212");


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



        public PrivatePlayerWrapper(PrivatePlayer player, uint periodTimeout)
        {
            _periodTimeout = periodTimeout;
            Player = player;
        }



        private async void OnPlayerTurn()
        {
            double r = TurnColor.R, g = TurnColor.G, b = TurnColor.B;
            new Animation(val => BackgroundColor = new Color(r = val, g, b), r, IdledColor.R,
               Easing.Linear).Commit(Application.Current.MainPage, "RAnimation", 32U, _periodTimeout);
           
            new Animation(val => BackgroundColor = new Color(r, g = val, b), g, IdledColor.G,
                Easing.Linear).Commit(Application.Current.MainPage, "GAnimation", 32U, _periodTimeout);

            new Animation(val => BackgroundColor = new Color(r, g, b = val), b, IdledColor.B,
                Easing.Linear).Commit(Application.Current.MainPage, "BAnimation", 32U, _periodTimeout);

            _isAnimating = true;
        }

    }
}
