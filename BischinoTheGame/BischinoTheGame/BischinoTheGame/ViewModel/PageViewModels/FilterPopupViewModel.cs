using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller.Communication.Queries;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class FilterPopupViewModel : PageViewModel
    {
        private Command _resetCommand;
        public Command ResetCommand
        {
            get => _resetCommand;
            set => SetProperty(ref _resetCommand, value);
        }


        private RoomSearchQuery _query;
        public RoomSearchQuery Query
        {
            get => _query;
            set => SetProperty(ref _query, value);
        }


        public FilterPopupViewModel(RoomSearchQuery query)
        {
            Query = query;
            ResetCommand = new Command(_ => Reset());
        }

        private void Reset()
        {
            Query.Model.Name = null;
            Query.Model.MaxPlayers = null;
            Query.Model.MinPlayers = null;
        }
    }
}
