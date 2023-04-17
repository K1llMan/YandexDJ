using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

namespace Yandex.Dj.Services.ContentProviders.Common
{
    public class ContentProvider
    {
        #region Поля

        protected List<PlaylistInfo> playlists;
        protected DirectoryInfo appDir;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Тип провайдера
        /// </summary>
        public ProviderType Type { get; protected set; }

        /// <summary>
        /// Конфигурация провайдера
        /// </summary>
        public JObject Config { get; private set; }

        #endregion Свойства

        #region Перегружаемые функции

        public virtual void UpdatePlaylists()
        {

        }

        /// <summary>
        /// Получить список доступных плейлистов без треков
        /// </summary>
        public virtual List<PlaylistInfo> GetPlaylists()
        {
            return playlists;
        }

        /// <summary>
        /// Получить содержимое плейлиста
        /// </summary>
        public virtual Playlist GetPlaylist(string id)
        {
            return null;
        }

        /// <summary>
        /// Получить ссылку на трек
        /// </summary>
        public virtual string GetTrack(string id)
        {
            return string.Empty;
        }

        /// <summary>
        /// Получить содержимое трекп
        /// </summary>
        public virtual Stream GetTrackContent(string id)
        {
            return null;
        }

        #endregion Перегружаемые функции

        #region Основные функция

        public ContentProvider(JObject config)
        {
            Type = ProviderType.Unknown;
            Config = config;

            playlists = new List<PlaylistInfo>();
            appDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        }

        #endregion Основные функция
    }
}
