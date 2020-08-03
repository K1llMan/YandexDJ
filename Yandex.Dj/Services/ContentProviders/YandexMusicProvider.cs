using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services.ContentProviders.Common;
using Yandex.Music.Api.Models.Playlist;
using Yandex.Music.Client;
using Yandex.Music.Client.Extensions;

namespace Yandex.Dj.Services.ContentProviders
{
    public class YandexMusicProvider: ContentProvider
    {
        #region Поля

        private YandexMusicClient client;

        #endregion Поля

        #region Вспомогательные функции

        private void InitAPI()
        {
            client = new YandexMusicClient();

            // Авторизация
            client.Authorize(Config["token"].ToString());
        }

        private string GetCover(string url, int size)
        {
            return string.IsNullOrEmpty(url)
                ? string.Empty
                : $"http://{url.Replace("%%", $"{size}x{size}")}";
        }

        private void CachePlaylists()
        {
            playlists = new List<PlaylistInfo>();

            // Кэширование плейлистов
            client.GetFavorites()
                .ForEach(p => {
                    playlists.Add(new PlaylistInfo {
                        Type = ProviderType.Yandex,
                        ID = p.GetKey().ToString(),
                        Title = p.Title,
                        Cover = GetCover(p.OgImage, 100)
                    });
                });

            // Персональные плейлисты
            client.GetPersonalPlaylists()
                .ForEach(p => {
                    playlists.Add(new PlaylistInfo {
                        Type = ProviderType.Yandex,
                        ID = p.GetKey().ToString(),
                        Title = p.Title,
                        Cover = GetCover(p.OgImage, 100)
                    });
                });
        }

        #endregion Вспомогательные функции

        #region Перегружаемые функции

        public override void UpdatePlaylists()
        {
            CachePlaylists();
        }

        public override Playlist GetPlaylist(string id)
        {
            string[] keys = id.Split(':');

            YPlaylist playlist = client.GetPlaylist(keys[0], keys[1]);

            return new Playlist {
                Type = ProviderType.Yandex,
                ID = playlist.GetKey().ToString(),
                Title = playlist.Title,
                Cover = GetCover(playlist.OgImage, 100),
                Tracks = playlist.Tracks.Select(t => new Track {
                    Type = ProviderType.Yandex,
                    ID = t.Track.GetKey().ToString(),
                    Artist = t.Track.Artists.Count > 0 ? t.Track.Artists.First().Name : string.Empty,
                    Title = t.Track.Title,
                    Cover = GetCover(t.Track.CoverUri, 100),
                    Gain = t.Track.Normalization.Gain
                })
                .ToArray()
            };
        }

        public override string GetTrack(string id)
        {
            return client.GetTrack(id).GetLink();
        }

        #endregion Перегружаемые функции

        #region Основные функции

        public YandexMusicProvider(JObject config) : base(config)
        {
            Type = ProviderType.Yandex;

            InitAPI();

            CachePlaylists();
        }

        #endregion Основные функции
    }
}
