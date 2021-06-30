using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller.Communication.Queries;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class FilterPopupViewModel : PageViewModel
    {
        // Initialization
        public FilterPopupViewModel(RoomSearchQuery query)
        {
            Query = query;
            ResetCommand = NewCommand(Reset);
        }

        // Commands
        public IAsyncCommand ResetCommand { get; }


        // Properties
        public RoomSearchQuery Query { get; }


        // Commands Handlers
        private void Reset()
        {
            Query.Model.Name = null;
            Query.Model.MaxPlayers = null;
            Query.Model.MinPlayers = null;
        }
    }
}
