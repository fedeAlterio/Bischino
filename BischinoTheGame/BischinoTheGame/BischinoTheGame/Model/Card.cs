using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.Controller;
using BischinoTheGame.ViewModel;
using Rooms.Controller;

namespace BischinoTheGame.Model
{
    public class Card : ViewModelBase
    {
        public const double Ratio = 2.7 / 4.0;


        private int _value;
        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }


        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public string IconName => AppController.Settings.GetCardIcon(Name);


        private string _owner;
        public string Owner
        {
            get => _owner;
            set => SetProperty(ref _owner, value);
        }


        private bool _isPaolo;
        public bool IsPaolo
        {
            get => _isPaolo;
            set => SetProperty(ref _isPaolo, value);
        }
    }
}
