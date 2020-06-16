﻿using System.Collections.Generic;

using Yandex.Music.Api.Common;
using Yandex.Music.Api.Common.YPlaylist;
using Yandex.Music.Api.Common.YTrack;

namespace Yandex.Dj.Services
{
    public class YandexMusicUser
    {
        #region Поля

        #endregion Поля

        #region События

        // Событие записи в лог
        public class UpdateCurrentSongEventArgs
        {
            public string Name { get; internal set; }
        }

        public delegate void UpdateCurrentSongHandler(UpdateCurrentSongEventArgs e);
        public event UpdateCurrentSongHandler UpdateCurrentSongEvent;

        #endregion События

        #region Свойства

        /// <summary>
        /// Текущая проигрываемая композиция
        /// </summary>
        public string CurrentSong { get; private set; }

        /// <summary>
        /// Текущий плейлист
        /// </summary>
        public List<YTrack> CurrentPlaylist { get; set; }

        /// <summary>
        /// Созданные плейлисты
        /// </summary>
        public List<YPlaylist> Playlists { get; set; }

        /// <summary>
        /// Хранилище
        /// </summary>
        public YAuthStorage Storage { get; set; }

        #endregion Свойства

        #region Основные функции

        public void UpdateCurrentSong(string song)
        {
            CurrentSong = song;

            UpdateCurrentSongEvent?.Invoke(new UpdateCurrentSongEventArgs {
                Name = song
            });
        }

        public YandexMusicUser()
        {
            CurrentPlaylist = new List<YTrack>();
            Playlists = new List<YPlaylist>();
            Storage = new YAuthStorage();
        }

        #endregion Основные функции
    }
}
