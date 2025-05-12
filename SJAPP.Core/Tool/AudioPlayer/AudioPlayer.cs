
// -------------------------------
// Project: AudioPlayer
// File: AudioPlayer.cs
// -------------------------------
using SJAPP.Core.Services.AudioPlayer;
using System;
using System.IO;
using System.Media;
using WMPLib;

namespace SJAPP.Core.Tool.AudioPlayer
{
    /// <summary>
    /// Implements IAudioPlayer using Windows Media Player COM for broad format support.
    /// </summary>
    public class AudioPlayer : IAudioPlayerService, IDisposable
    {
        private WindowsMediaPlayer _player;
        private bool _disposed;

        public AudioPlayer()
        {
            _player = new WindowsMediaPlayer();
        }

        public bool IsPlaying => _player.playState == WMPPlayState.wmppsPlaying;

        public void Play(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Audio file not found.", filePath);

            _player.URL = filePath;
            _player.controls.play();
        }

        public void Stop()
        {
            if (IsPlaying)
                _player.controls.stop();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _player.controls.stop();
                _player.close();
                _player = null;
                _disposed = true;
            }
        }
    }
}


