using System;
using System.Collections.Generic;
using System.Text;
using BischinoTheGame.ViewModel;
using Xamarin.Essentials;

namespace BischinoTheGame.Model.Settings
{
    public class SettingsManager : ViewModelBase
    {
        public event EventHandler<DeckType> DeckChanged;
        public DeckType DeckType
        {
            get => (DeckType) Preferences.Get(nameof(DeckType), (int) DeckType.C);
            set
            {
                Preferences.Set(nameof(DeckType), (int) value);
                DeckChanged?.Invoke(this, value);
            }
        }


        public bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }


        public bool AudioOn
        {
            get => Preferences.Get(nameof(AudioOn), true);
            set => Preferences.Set(nameof(AudioOn), value);
        }


        public bool SoundEffectsOn
        {
            get => Preferences.Get(nameof(SoundEffectsOn), true);
            set
            {
                Preferences.Set(nameof(SoundEffectsOn), value);
                OnPropertyChanged();
            }
        }


        public string GetCardIcon(string number) => GetCardIcon(number, DeckType);
        public string GetCardIcon(string number, DeckType deckType) => deckType switch
        {
            DeckType.A => $"c{number}",
            DeckType.B => $"D{number}",
            DeckType.C => $"E{number}"
        };
}
}
