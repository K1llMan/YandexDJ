﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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

        /// <summary>
        /// Управление данными через сокеты
        /// </summary>
        public Broadcast Broadcast { get; private set; }

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
                        case ProviderType.Local:
                            contentProviders.Add(new LocalProvider((JObject)provider.Value));
                            break;
                    }
                }
        }

        #endregion Вспомогательные функции

        #region Основные функции

        #region Взаимодействие с провайдерами

        public void UpdatePlaylists(ProviderType type = ProviderType.Unknown)
        {
            if (type == ProviderType.Unknown) {
                contentProviders.ForEach(p => p.UpdatePlaylists());
                return;
            }

            contentProviders
                .FirstOrDefault(p => p.Type == type)
                ?.UpdatePlaylists();
        }

        public List<Dictionary<string, object>> GetPlaylists()
        {
            return contentProviders
                .Select(p => new Dictionary<string, object> {
                    { "group",  p.Type },
                    { "playlists", p.GetPlaylists() } 
                    })
                .ToList();
        }

        public Playlist GetPlaylist(ProviderType type, string id)
        {
            return contentProviders
                .FirstOrDefault(p => p.Type == type)
                ?.GetPlaylist(id);
        }

        public string GetMusicLink(ProviderType type, string id)
        {
            ContentProvider provider = contentProviders.FirstOrDefault(p => p.Type == type);
            if (provider == null)
                return string.Empty;

            return provider.GetTrack(id);
        }

        #endregion Взаимодействие с провайдерами

        #region Обновление данных

        public void UpdateCurrentSong(string song)
        {
            CurrentTrack = song;

            UpdateCurrentSongEvent?.Invoke(new UpdateCurrentTrackEventArgs {
                Name = song
            });
        }

        #endregion Обновление данных

        #region Формирование контента

        public FileStream GetPlaylistCover(string id)
        {
            LocalProvider provider = (LocalProvider) contentProviders.FirstOrDefault(p => p.Type == ProviderType.Local);

            return provider?.GetPlaylistCover(id);
        }

        public FileStream GetTrackCover(string id)
        {
            LocalProvider provider = (LocalProvider)contentProviders.FirstOrDefault(p => p.Type == ProviderType.Local);

            return provider?.GetTrackCover(id);
        }

        public FileStream GetTrackContent(string id)
        {
            LocalProvider provider = (LocalProvider)contentProviders.FirstOrDefault(p => p.Type == ProviderType.Local);

            return provider?.GetTrackContent(id);
        }

        #endregion Формирование контента

        #region Обработчики сокетов

        private void InitBroadcastHandlers()
        {
            // Подписка на событие смены трека
            UpdateCurrentSongEvent += eventArgs =>  {
                Broadcast.Send(new BroadcastEvent {
                    Event = "updateSong",
                    Data = eventArgs.Name
                });
            };

            Func<object, string> serializeMessage = o => JsonConvert.SerializeObject(o, Formatting.None,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            Broadcast.On("getCurrentSong", (wrapper, o) => {
                wrapper.Send(serializeMessage(new BroadcastEvent {
                    Event = "updateSong",
                    Data = CurrentTrack
                }));
            });
        }

        #endregion Обработчики сокетов

        public StreamingService()
        {
            contentProviders = new List<ContentProvider>();
            Broadcast = new Broadcast();

            InitProviders();
            InitBroadcastHandlers();
        }

        #endregion Основные функции
    }
}
