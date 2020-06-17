using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services.ContentProviders;
using Yandex.Dj.Services.ContentProviders.Common;

namespace Yandex.Dj.Services
{
    public class StreamingService
    {
        #region Поля

        private List<ContentProvider> contentProviders;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Текущий трек
        /// </summary>
        public string CurrentTrack { get; private set; }

        #endregion Свойства

        #region События

        // Событие записи в лог
        public class UpdateCurrentTrackEventArgs
        {
            public string Name { get; internal set; }
        }

        public delegate void UpdateCurrentTrackHandler(UpdateCurrentTrackEventArgs e);
        public event UpdateCurrentTrackHandler UpdateCurrentSongEvent;

        #endregion События

        #region Вспомогательные функции

        private void InitProviders()
        {
            JObject settings = JsonCommon.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"));

            JObject providers = (JObject)settings["providers"];
            if (!providers.IsNullOrEmpty())
                foreach (JProperty provider in providers.Properties()) {
                    switch (Enum.Parse<ProviderType>(provider.Name, true)) {
                        case ProviderType.Yandex:
                            contentProviders.Add(new YandexMusicProvider((JObject)provider.Value));
                            break;
                    }
                }
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public List<PlaylistInfo> GetPlaylists()
        {
            return contentProviders
                .SelectMany(p => p.GetPlaylists())
                .ToList();
        }

        public Playlist GetPlaylist(ProviderType type, string id)
        {
            ContentProvider provider = contentProviders.FirstOrDefault(p => p.Type == type);
            if (provider == null)
                return null;

            return provider.GetPlaylist(id);
        }

        public string GetMusicLink(ProviderType type, string id)
        {
            ContentProvider provider = contentProviders.FirstOrDefault(p => p.Type == type);
            if (provider == null)
                return string.Empty;

            return provider.GetTrack(id);
        }

        public void UpdateCurrentSong(string song)
        {
            CurrentTrack = song;

            UpdateCurrentSongEvent?.Invoke(new UpdateCurrentTrackEventArgs
            {
                Name = song
            });
        }

        public StreamingService()
        {
            contentProviders = new List<ContentProvider>();

            InitProviders();
        }

        #endregion Основные функции
    }
}
