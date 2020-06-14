using System.Collections.Generic;

namespace Yandex.Dj.Services
{
    public class YandexMusicUserCollection
    {
        #region Поля

        private Dictionary<string, YandexMusicUser> users;

        #endregion Поля

        #region Свойства

        public YandexMusicUser this[string user]
        {
            get
            {
                return users.ContainsKey(user)
                    ? users[user]
                    : null;
            }
        }

        #endregion Свойства

        #region Основные функции

        public void Add(string key, YandexMusicUser user)
        {
            if (!users.ContainsKey(key))
                users.Add(key, user);
        }

        public void Remove(string key)
        {
            if (users.ContainsKey(key))
                users.Remove(key);
        }

        public YandexMusicUserCollection()
        {
            users = new Dictionary<string, YandexMusicUser>();
        }

        #endregion Основные функции
    }
}
