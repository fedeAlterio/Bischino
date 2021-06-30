using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BischinoTheGame.Model;
using Xamarin.Forms;

namespace BischinoTheGame.ViewModel.PageViewModels
{
    public class CardWrapper : ViewModelBase
    {
        private static int _instanceCounter;
        private int _counter;



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



        public CardWrapper(Card card)
        {
            Card = card;
            Scale = 1;
            _counter = _instanceCounter++;
        }

        public async Task ScaleTo(double finalZoom)
        {
            var taskSource = new TaskCompletionSource<bool>();
            new Animation(val => Scale = val, Scale, finalZoom)
                .Commit(Application.Current.MainPage, $"{nameof(CardWrapper)}{_counter++}", 32, 400, Easing.Linear,
                    (_, val) => taskSource.SetResult(val));
            await taskSource.Task;
        }
    }
}
