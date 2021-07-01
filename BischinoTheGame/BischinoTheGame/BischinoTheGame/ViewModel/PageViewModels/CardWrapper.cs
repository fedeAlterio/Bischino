using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Helpers;
using BischinoTheGame.Model;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class CardWrapper : ViewModelBase
    {
        private static int _instanceCounter;
        private int _counter;

        // Initialization
        public CardWrapper(Card card)
        {
            Card = card;
            Scale = 1;
            _counter = _instanceCounter++;
        }

        
        // Properties
        private Card _card;
        public Card Card
        {
            get => _card;
            set => SetProperty(ref _card, value);
        }


        private double _scale;
        public double Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

      

        public Task ScaleTo(double finalZoom)
        {
            return AnimationManager.AsyncAnimation(Scale, finalZoom, val => Scale = val, 32, 450, Easing.Linear);
        }
    }
}
