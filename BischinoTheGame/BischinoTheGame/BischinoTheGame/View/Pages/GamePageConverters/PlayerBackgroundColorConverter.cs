using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BischinoTheGame.Model;
using Xamarin.Forms;

namespace BischinoTheGame.View.Pages.GamePageConverters
{
    public class PlayerBackgroundColorConverter : IValueConverter
    {
        public Color PlayingColor { get; set; }
        public Color IdledColor { get; set; }   
        public Color TurnColor { get; set; }
        public Color HashLostColor { get; set; }



        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return IdledColor;
            var player = value as PrivatePlayer;

            if (player.IsTurn)
                return TurnColor;

            if (player.IsIdled)
                return IdledColor;

            if (player.HasLost)
                return HashLostColor;

            return PlayingColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
