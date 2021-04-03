using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;

namespace Yandex.Dj.Services.Rocksmith
{
    public class RocksmithService
    {
        #region Поля

        private DirectoryInfo rocksmithDir;
        private char[] trackPartsSeparator = { '|' };

        #endregion Поля

        #region Свойства
        
        /// <summary>
        /// Конфигурация
        /// </summary>
        public RocksmithConfig Config { get; }

        /// <summary>
        /// Очередь треков
        /// </summary>
        public RocksmithQueue Queue { get; }

        #endregion Свойства

        #region Вспомогательные функции

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

            string message = Queue.Add(track);

            return message;
        }

        /// <summary>
        /// Удаление трека
        /// </summary>
        public void RemoveTrack(RocksmithTrack track)
        {
            Queue.RemoveTrack(track);
        }

        public RocksmithService()
        {
            rocksmithDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocksmith"));

            Config = JsonCommon.Load<RocksmithConfig>(Path.Combine(rocksmithDir.FullName, "rocksmith.json"));
            Queue = new RocksmithQueue(rocksmithDir.FullName, Config.UserLimit);
        }

        #endregion Основные функции
    }
}
