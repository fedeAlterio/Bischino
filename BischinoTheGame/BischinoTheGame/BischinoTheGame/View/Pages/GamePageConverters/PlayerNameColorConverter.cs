using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.Model;
using Rooms.Controller;
using Xamarin.Forms;

namespace BischinoTheGame.View.Pages.GamePageConverters
{
    public class PlayerNameColorConverter : IValueConverter
    {
        public Color StandardColor { get; set; }
        public Color CurrentPlayerStandardColor { get; set; }
        public Color IdledColor { get; set; }
        public Color TurnColor { get; set; }
        public Color CurrentPayerTurnColor { get; set; }
        public Color LostColor { get; set; }
        public Color CurrentPlayerLostColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;

            var player = value as PrivatePlayer;
            var currentPlayer = AppController.Navigation.GameNavigation.LoggedPlayer;

            if (player.IsIdled)
                return IdledColor;

            Color ret;

            if (player.Name == currentPlayer.Name)
                if (player.IsTurn)
                    ret = CurrentPayerTurnColor;
                else if (player.HasLost)
                    ret = CurrentPlayerLostColor;
                else
                    ret = CurrentPlayerStandardColor;
            else
                if (player.IsTurn)
                    ret = TurnColor;
                else if (player.HasLost)
                    ret = LostColor;
                else
                    ret = StandardColor;

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
