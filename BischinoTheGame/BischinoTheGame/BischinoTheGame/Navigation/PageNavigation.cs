using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Rooms.Controller.Navigation
{
    public abstract class PageNavigationBase
    {
        public static INavigation Navigation { get; set; }

        protected PageNavigationBase()
        {
            if(Navigation is null)
                throw new Exception($"{nameof(PageNavigationBase)}.{nameof(Navigation)} not initialized");
        }
    }
}
