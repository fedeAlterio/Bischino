using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BischinoTheGame.Model;
using Xamarin.Forms;

namespace BischinoTheGame.View.Pages
{
    public class CardToPaoloColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Card card) || !card.IsPaolo)
                return Color.Transparent;

            return card.Value > 39 ? Color.DarkRed : Color.FromHex("#204051");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
