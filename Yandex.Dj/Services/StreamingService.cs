using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services.Bot;
using Yandex.Dj.Services.ContentProviders;
using Yandex.Dj.Services.ContentProviders.Common;
using Yandex.Dj.Services.Rocksmith;
using Yandex.Dj.Services.Streaming;
using Yandex.Dj.Services.Widgets;

namespace Yandex.Dj.Services
{
    public class StreamingService: IDisposable
    {
        #region Поля

        private List<ContentProvider> contentProviders;
        private WidgetsScheme currentScheme;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Список доступных схем
        /// </summary>
        public List<WidgetsScheme> WidgetsSchemes { get; private set; }

        /// <summary>
        /// Схема виджетов
        /// </summary>
        public WidgetsScheme WidgetsScheme {
            get { return currentScheme; }
            set
            {
                currentScheme = value;
                UpdateCurrentSchemeEvent?.Invoke(new UpdateCurrentSchemeEventArgs {
                    Scheme = value
                });
            }
        }

        /// <summary>
        /// Текущий трек
        /// </summary>
        public string CurrentTrack { get; private set; }

        /// <summary>
        /// Управление данными через сокеты
        /// </summary>
        public Broadcast Broadcast { get; }

        /// <summary>
        /// Бот
        /// </summary>
        public BotService Bot { get; set; }

        /// <summary>
        /// Rocksmith
        /// </summary>
        public RocksmithService Rocksmith { get; set; }

        /// <summary>
        /// Управление Youtube
        /// </summary>
        public YoutubeConnector Youtube { get; }

        /// <summary>
        /// Управление Twitch
        /// </summary>
        public TwitchConnector Twitch { get; }

        #endregion Свойства

        #region События

        // Событие обновления трека
        public class UpdateCurrentTrackEventArgs
        {
            public string Name { get; internal set; }
        }

        public delegate void UpdateCurrentTrackHandler(UpdateCurrentTrackEventArgs e);
        public event UpdateCurrentTrackHandler UpdateCurrentSongEvent;

        // Событие обновления схемы
        public class UpdateCurrentSchemeEventArgs
        {
            public WidgetsScheme Scheme{ get; internal set; }
        }

        public delegate void UpdateCurrentSchemeHandler(UpdateCurrentSchemeEventArgs e);
        public event UpdateCurrentSchemeHandler UpdateCurrentSchemeEvent;

        #endregion События

        #region Вспомогательные функции

        private void InitProviders(JObject providers)
        {
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

        private void InitWidgetsSchemes(string current)
        {
            WidgetsSchemes = new List<WidgetsScheme>();

            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "widgets");
            string[] files = Directory.GetFiles(fileName, "*.json");
            foreach (string file in files)
                WidgetsSchemes.Add(JsonCommon.Load<WidgetsScheme>(file));

            WidgetsScheme = WidgetsSchemes.FirstOrDefault(s => s.Name == current);
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

        public FileStream GetTrackContent(ProviderType type, string id)
        {
            ContentProvider provider = contentProviders.FirstOrDefault(p => p.Type == type);

            return provider?.GetTrackContent(id);
        }

        #endregion Формирование контента

        #region Обработчики сокетов

        private void InitBroadcastHandlers()
        {
            // Подписка на событие смены трека
            UpdateCurrentSongEvent += async eventArgs =>  {
                await Broadcast.Send(new BroadcastEvent {
                    Event = "updateSong",
                    Data = eventArgs.Name
                });
            };

            // Подписка на событие смены схемы
            UpdateCurrentSchemeEvent += async eventArgs => {
                await Broadcast.Send(new BroadcastEvent {
                    Event = "updateScheme",
                    Data = eventArgs.Scheme
                });
            };

            // События бота
            Bot.TextToSpeechEvent += async eventArgs => {
                await Broadcast.Send(new BroadcastEvent {
                    Event = "speech",
                    Data = eventArgs.FileName
                });
            };

            Bot.SoundMessageEvent += async eventArgs => {
                await Broadcast.Send(new BroadcastEvent {
                    Event = "sound",
                    Data = eventArgs.FileName
                });
            };

            // События Rocksmith
            Rocksmith.TrackAddEvent += async eventArgs => {
                await Broadcast.Send(new BroadcastEvent {
                    Event = "addRocksmithTrack",
                    Data = eventArgs.Track
                });
            };

            Rocksmith.TrackRemoveEvent += async eventArgs => {
                await Broadcast.Send(new BroadcastEvent {
                    Event = "removeRocksmithTrack",
                    Data = eventArgs.Track
                });
            };

            // Подписка на события
            Func<object, string> serializeMessage = o => JsonConvert.SerializeObject(o, Formatting.None,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            Broadcast.On("getCurrentSong", async (wrapper, o) => {
                await wrapper.Send(serializeMessage(new BroadcastEvent {
                    Event = "updateSong",
                    Data = CurrentTrack
                }));
            });

            Broadcast.On("getCurrentScheme", async (wrapper, o) => {
                await wrapper.Send(serializeMessage(new BroadcastEvent {
                    Event = "updateScheme",
                    Data = WidgetsScheme
                }));
            });

            Broadcast.On("getRocksmithTracks", async (wrapper, o) => {
                await wrapper.Send(serializeMessage(new BroadcastEvent {
                    Event = "updateRocksmithTracks",
                    Data = Rocksmith.TrackList
                }));
            });
        }

        #endregion Обработчики сокетов

        public StreamingService(BotService bot, RocksmithService rocksmith)
        {
            // Бот
            Bot = bot;
            Rocksmith = rocksmith;

            // Настройки приложения
            JObject settings = JsonCommon.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"));

            contentProviders = new List<ContentProvider>();
            Broadcast = new Broadcast();

            JToken streamingConfigs = settings["streaming"];
            if (!streamingConfigs.IsNullOrEmpty()) {
                Twitch = new TwitchConnector(Bot, (JObject)streamingConfigs["twitch"]);
                Youtube = new YoutubeConnector(bot, (JObject)streamingConfigs["youtube"]);
            }

            InitProviders((JObject)settings["providers"]);
            InitWidgetsSchemes(settings["scheme"].ToString());
            InitBroadcastHandlers();
        }

        #endregion Основные функции

        public void Dispose()
        {
            if (Youtube != null) {

            }
        }
    }
}
