using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.ViewModel;
using Xamarin.Forms;

namespace BischinoTheGame.View.ViewElements
{
    public class ColorWrapper<T> : ViewModelBase
    {
        private T _model;
        public T Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }



        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
    }
}
