using BischinoTheGame.Controller;
using BischinoTheGame.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public abstract class PageViewModel : ViewModelBase
    {
        private readonly IAppNavigation _appNavigation;
        protected const string ErrorDefault = "An error occurred";
        protected const string ErrorTitle = "Warning";


        // Initialization
        public PageViewModel()
        {
            _appNavigation = AppController.Navigation;
        }

        public PageViewModel(IAppNavigation appNavigation)
        {
            _appNavigation = appNavigation;
        }





        #region Bindable Properties        

        private bool _isPageEnabled = true;
        public bool IsPageEnabled
        {
            get => _isPageEnabled;
            protected set => SetProperty(ref _isPageEnabled, value);
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion





        // Events
        protected async void HandleException(Exception e)
        {            
            await _appNavigation.DisplayAlert(ErrorTitle, e.Message);
        }




        // Helpers
        protected IAsyncCommand NewCommand(Func<Task> execute, Func<bool> canExectute = null, bool allowMultipleExecutions = false)
            => new AsyncCommand(execute, canExectute ?? (() => true), HandleException, true, allowMultipleExecutions);

        protected IAsyncCommand NewCommand(Action execute, Func<bool> canExectute = null, bool allowMultipleExecutions = false)
        => new AsyncCommand(async () => execute(), canExectute ?? (() => true), HandleException, true, allowMultipleExecutions);

        protected IAsyncCommand<T> NewCommand<T>(Func<T, Task> execute, Func<bool> canExecute = null, bool allowMultipleExecutions = false)
            => new AsyncCommand<T>(execute, canExecute ?? (() => true), HandleException, true, allowMultipleExecutions);

        protected IAsyncCommand<T> NewCommand<T>(Action<T> execute, Func<bool> canExecute = null, bool allowMultipleExecutions = false)
           => new AsyncCommand<T>(async x => execute(x), canExecute ?? (() => true), HandleException, true, allowMultipleExecutions);
    }
}
