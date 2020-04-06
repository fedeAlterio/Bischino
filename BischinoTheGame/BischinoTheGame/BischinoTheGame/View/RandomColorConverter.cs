using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Bischino.Extensions;
using Xamarin.Forms;

namespace BischinoTheGame.View
{
    public class RandomColorConverter : IValueConverter
    {
        public static readonly IList<Color> Colors = new List<Color>
        {
            Color.DarkRed,
            Color.FromHex("#204051"),
            Color.DarkOrange,
            Color.FromHex("#2f3640"),
            Color.Brown,
            Color.BlueViolet, 
            Color.Goldenrod,
            Color.Olive
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Colors.Shuffle();
            return Colors[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
