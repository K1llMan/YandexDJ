using System.Collections.Generic;

using Yandex.Music.Api.Common;
using Yandex.Music.Api.Common.YPlaylist;
using Yandex.Music.Api.Common.YTrack;

namespace Yandex.Dj.Services
{
    public class YandexMusicUser
    {
        #region Поля

        #endregion Поля

        #region Свойства

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

        public YandexMusicUser()
        {
            CurrentPlaylist = new List<YTrack>();
            Playlists = new List<YPlaylist>();
            Storage = new YAuthStorage();
        }

        #endregion Основные функции
    }
}
