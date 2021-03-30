using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services.Bot;

namespace Yandex.Dj.Services.Rocksmith
{
    public class RocksmithService
    {
        #region Поля

        private DirectoryInfo rocksmithDir;
        private char[] trackPartsSeparator = { '|' };
        private int userLimit = 2;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Список треков
        /// </summary>
        public List<RocksmithTrack> TrackList { get; }

        /// <summary>
        /// Список отложенных треков
        /// </summary>
        public List<RocksmithTrack> BackList { get; }


        #endregion Свойства

        #region События

        // Событие добавления/удаления трека
        public class TrackEventArgs
        {
            public RocksmithTrack Track { get; internal set; }
        }

        public delegate void TrackHandler(TrackEventArgs e);
        public event TrackHandler TrackAddEvent;

        public event TrackHandler TrackRemoveEvent;

        #endregion События

        #region Вспомогательные функции

        private void Init()
        {
            JObject settings = JsonCommon.Load(Path.Combine(rocksmithDir.FullName, "rocksmith.json"));
            if (!settings.IsNullOrEmpty()) {
                if (!settings["separators"].IsNullOrEmpty())
                    trackPartsSeparator = settings["separators"].Select(t => t.ToString()[0]).ToArray();
                if (!settings["userLimit"].IsNullOrEmpty())
                    userLimit = Convert.ToInt32(settings["userLimit"].ToString());
            }
        }

        #endregion Вспомогательные функции

        #region Основные функции

        /// <summary>
        /// Добавление трека
        /// </summary>
        public string AddTrack(string data, string user)
        {
            List<string> parts = data.Split(trackPartsSeparator)
                .Select(p => p.Trim())
                .ToList();

            for (int i = 0; i < 4 - parts.Count; i++)
                parts.Add("Any");

            RocksmithTrack track = new() {
                // Ключ для добавления/удаления
                Key = new Guid(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()))).ToString(),
                Artist = parts[0],
                Name = parts[1],
                User = user,
                ArrangementType = parts[2]
            };

            // Если список треков от пользователя достиг лимита, то добавляем в отложенную очередь
            int count = TrackList.Count(t => t.User == user);
            if (count >= userLimit) {
                BackList.Add(track);

                int backListCount = BackList.Count(t => t.User == user);

                return $"{user}, трек добавлен в отложенную очередь, {count + backListCount} треков всего";
            }

            TrackList.Add(track);

            TrackAddEvent?.Invoke(new TrackEventArgs {
                Track = track
            });

            return $"{user}, трек добавлен.";
        }

        /// <summary>
        /// Удаление трека
        /// </summary>
        public void RemoveTrack(RocksmithTrack track)
        {
            if (!TrackList.Contains(track))
                return;

            TrackList.Remove(track);

            TrackRemoveEvent?.Invoke(new TrackEventArgs {
                Track = track
            });

            // После удаления из очереди проверяем треки, которые можно добавить из отложенных
            RocksmithTrack addTrack = BackList.FirstOrDefault(t => TrackList.Count(tt => tt.User == t.User) < userLimit);
            if (addTrack != null) {
                BackList.Remove(addTrack);
                TrackList.Add(addTrack);

                TrackAddEvent?.Invoke(new TrackEventArgs {
                    Track = addTrack
                });
            }
        }

        public RocksmithService()
        {
            TrackList = new List<RocksmithTrack>();
            // Треки для очереди сверх лимита
            BackList = new List<RocksmithTrack>();

            rocksmithDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocksmith"));

            Init();
        }

        #endregion Основные функции
    }
}
