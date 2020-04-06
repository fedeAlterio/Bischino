using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BischinoTheGame.Model;
using BischinoTheGame.ViewModel;
using Plugin.SimpleAudioPlayer;
using Rooms.Controller;

namespace BischinoTheGame.Controller
{
    public class AudioManager : ViewModelBase
    {

        private readonly ISimpleAudioPlayer _player;
        private string _currentTrack;
        private bool _backgroundPlayingStateBackup;
        public event EventHandler<bool> PlayingStateChanged;


        private readonly IList<string> _trackNames = new List<string>
        {
            "tea_music.mp3",
            "african_son.mp3",
            "magic.mp3",
            "conquistador.mp3"
        };


        public bool AudioOn
        {
            get => AppController.Settings.AudioOn;
            set
            {
                if (value)
                    _player.Play();
                else
                    _player.Pause();
                
                AppController.Settings.AudioOn = value;
                OnPropertyChanged();
                PlayingStateChanged?.Invoke(this, value);
            }
        }


        public void PlaySound(SoundEffect soundEffect)
        {
            if (!AppController.Settings.SoundEffectsOn)
                return;

            var stream = GetSoundEffect(soundEffect);
            var player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            player.Load(stream);
            player.Play();
        }


        public AudioManager()
        {
            _player = CrossSimpleAudioPlayer.Current;
            _currentTrack = _trackNames[0];
            LoadTrack();
            _player.PlaybackEnded += _player_PlaybackEnded;
        }

        private void _player_PlaybackEnded(object sender, EventArgs e)
        {
            if(AudioOn)
                PlayNext();
        }


        public void GoingBackground()
        {
            _backgroundPlayingStateBackup = _player.IsPlaying;
            _player.Pause();
        }

        public void OnResume()
        {
            if (_backgroundPlayingStateBackup)
                _player.Play();
            _backgroundPlayingStateBackup = false;
        }

        public void PlayNext()
        {
            AudioOn = false;
            NextTrack();
            LoadTrack();
            AudioOn = true;
        }

        public void PlayPrevious()
        {
            AudioOn = false;
            PreviousTrack();
            LoadTrack();
            AudioOn = true;
        }

        private void NextTrack()
        {
            _currentTrack = _currentTrack == _trackNames.Last()
                ? _trackNames[0]
                : _trackNames[_trackNames.IndexOf(_currentTrack) + 1];
        }


        private void PreviousTrack()
        {
            _currentTrack = _currentTrack == _trackNames.First()
                ? _trackNames.Last()
                : _trackNames[_trackNames.IndexOf(_currentTrack) - 1];
        }

        private void LoadTrack()
        {
            var stream = GetAudioStream(_currentTrack);
            _player.Load(stream);
        }

        private static Stream GetAudioStream(string audioName)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var audioStream = assembly.GetManifestResourceStream($"BischinoTheGame.Sfx.{audioName}");
            return audioStream;
        }

        private Stream GetSoundEffect(SoundEffect soundEffect) => GetAudioStream( soundEffect switch
        {
            SoundEffect.Pop => "pop.flac",
            SoundEffect.Win => "win.wav",
            SoundEffect.Disconnected => "disconnected.wav",
            SoundEffect.Coin => "coin.wav",
        });
    }
}
