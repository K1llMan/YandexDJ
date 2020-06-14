using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Yandex.Music.Api;
using Yandex.Music.Api.Common;
using Yandex.Music.Api.Common.YPlaylist;

namespace Yandex.Dj.Services
{
    public class YandexMusicService
    {
        #region Поля

        
        private string testUser = "k1llman";


        #endregion Поля

        #region Свойства

        public YandexMusicApi API { get; }

        public YandexMusicUserCollection Users { get; }

        #endregion Свойства

        #region Вспомогательные функции

        private YandexMusicUser InitUser(YandexMusicApi api)
        {
            JObject settings;
            using (var stream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json"), FileMode.Open))
                using (var reader = new StreamReader(stream))
                {
                    string data = reader.ReadToEnd();
                    settings = JObject.Parse(data);
                }

            YandexMusicUser user = new YandexMusicUser();

            // Авторизация
            API.UserAPI.Authorize(user.Storage, settings["token"].ToString());

            // Кэширование плейлистов
            API.PlaylistAPI.Favorites(user.Storage)
                .ForEach(p => {
                    user.Playlists.Add(API.PlaylistAPI.Get(user.Storage, p));
                });

            // Персональные плейлисты
            API.PlaylistAPI.MainPagePersonal(user.Storage)
                .ForEach(p => {
                    user.Playlists.Add(API.PlaylistAPI.Get(user.Storage, p));
                });

            return user;
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public string GetMusicLink(YandexMusicUser user, string key)
        {
            return API.TrackAPI.GetFileLink(user.Storage, key);
        }

        public YandexMusicService()
        {
            API = new YandexMusicApi();
            Users = new YandexMusicUserCollection();

            Users.Add(testUser, InitUser(API));
        }

        #endregion Основные функции
    }
}
