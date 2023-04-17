using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services.ContentProviders.Common;

namespace Yandex.Dj.Services.ContentProviders
{
    public class LocalProvider: ContentProvider
    {
        #region Классы

        public class LocalTrack
        {
            /// <summary>
            /// Идентификатор
            /// </summary>
            public string ID { get; set; }
            /// <summary>
            /// Путь к обложке
            /// </summary>
            public string Cover { get; set; }
            /// <summary>
            /// Исполнитель
            /// </summary>
            public string Artist { get; set; }
            /// <summary>
            /// Название
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// Путь к данным
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// Усиление
            /// </summary>
            public double Gain { get; set; } = 12;
        }

        private class LocalPlaylist
        {
            /// <summary>
            /// Идентификатор
            /// </summary>
            public string ID { get; set; }
            /// <summary>
            /// Путь к обложке
            /// </summary>
            public string Cover { get; set; }
            /// <summary>
            /// Название
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// Список треков в плейлисте
            /// </summary>
            public LocalTrack[] Tracks { get; set; }
        }

        #endregion Классы

        #region Поля

        private string playlistsDir;
        private List<LocalPlaylist> localPlaylists;

        #endregion Поля

        #region Вспомогательные функции

        public void CachePlaylists()
        {
            localPlaylists = new List<LocalPlaylist>();
            playlists = new List<PlaylistInfo>();

            FileInfo[] files = new DirectoryInfo(playlistsDir).GetFiles("*.json");
            if (files.Length == 0)
                return;

            int id = 0;
            foreach (FileInfo file in files) {
                LocalPlaylist playlist = JsonCommon.Load<LocalPlaylist>(file.FullName);
                playlist.ID = $"{id}";

                int trackId = 0;
                foreach (LocalTrack track in playlist.Tracks)
                    track.ID = $"{id}:{trackId++}";

                localPlaylists.Add(playlist);

                id++;
            }

            localPlaylists.ForEach(p => {
                playlists.Add(new PlaylistInfo {
                    Type = ProviderType.Local,
                    ID = p.ID,
                    Title = p.Title,
                    Cover = $"api/content/playlistCover?id={p.ID}"
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
            LocalPlaylist playlist = localPlaylists.FirstOrDefault(p => p.ID == id);
            if (playlist == null)
                return null;

            return new Playlist {
                Type = ProviderType.Local,
                ID = playlist.ID,
                Title = playlist.Title,
                Cover = $"api/content/playlistCover?id={playlist.ID}",
                Tracks = playlist.Tracks.Select(t => new Track {
                    Type = ProviderType.Local,
                    ID = t.ID,
                    Artist = t.Artist,
                    Title = t.Title,
                    Cover = $"api/content/trackCover?id={t.ID}",
                    Gain = t.Gain
                }).ToArray()
            };
        }

        public override string GetTrack(string id)
        {
            return $"api/content/track?type=local&id={id}";
        }

        #endregion Перегружаемые функции

        #region Основные функции

        #region Работа с содержимым локального провайдера

        public FileStream GetPlaylistCover(string id)
        {
            string path = localPlaylists.FirstOrDefault(p => p.ID == id)?.Cover;
           
            return string.IsNullOrEmpty(path) || !File.Exists(path) 
                ? null 
                : new FileStream(path, FileMode.Open);
        }

        public FileStream GetTrackCover(string id)
        {
            string[] parts = id.Split(':');

            string path = localPlaylists
                .FirstOrDefault(p => p.ID == parts[0])
                ?.Tracks.FirstOrDefault(t => t.ID == id)
                ?.Cover;

            return string.IsNullOrEmpty(path) 
                ? null 
                : new FileStream(path, FileMode.Open);
        }

        public override FileStream GetTrackContent(string id)
        {
            string pId = id.Split(':')[0];

            string path = localPlaylists
                .FirstOrDefault(p => p.ID == pId)
                ?.Tracks.FirstOrDefault(t => t.ID == id)
                ?.Path;

            return string.IsNullOrEmpty(path)
                ? null
                : new FileStream(path, FileMode.Open);
        }

        #endregion Работа с содержимым локального провайдера

        public LocalProvider(JObject config) : base(config)
        {
            Type = ProviderType.Local;

            playlistsDir = Path.Combine(appDir.FullName, config["path"].ToString());

            CachePlaylists();
        }

        #endregion Основные функции
    }
}
