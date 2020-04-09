using System;
using System.Collections.Generic;
using System.Text;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public abstract class PageViewModel : ViewModelBase
    {
        protected const string ErrorDefault = "An error occurred, check your internet connection";
        protected const string ErrorTitle = "Warning";

        private bool _isPageEnabled = true;
        public bool IsPageEnabled
        {
            get => _isPageEnabled;
            protected set
            {
                if (SetProperty(ref _isPageEnabled, value))
                    if (!_isPageEnabled)
                        DisablePage();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }


        private bool _isPageLoaded = true;
        public bool IsPageLoaded
        {
            get => _isPageLoaded;
            set => SetProperty(ref _isPageLoaded, value);
        }


        protected virtual void DisablePage() { }
    }
}
