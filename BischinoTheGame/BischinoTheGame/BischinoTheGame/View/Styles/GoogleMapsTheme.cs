using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Rooms.View.Styles
{
    public static class GoogleMapsTheme
    {
        public static readonly Color LineColor = new Color(165, 194, 206);
        public static string GetThemeJson()
        {
            var ret = "";
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Rooms.View.Styles.MapsTheme.json");
            if (stream != null)
                using (var reader = new StreamReader(stream))
                {
                    ret = reader.ReadToEnd();
                }

            return ret;
        }
    }
}
