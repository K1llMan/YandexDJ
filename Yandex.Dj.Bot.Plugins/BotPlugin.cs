using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

using Yandex.Dj.CommonServices.WebSocket;

namespace Yandex.Dj.Bot.Plugins
{
    /// <summary>
    /// Базовый класс реализации плагина
    /// </summary>
    public class BotPlugin<T> where T: BotPluginSettings
    {
        #region Свойства

        public string WorkDir { get; set; }

        /// <summary>
        /// Настройки плагина
        /// </summary>
        public T Settings { get; set; }

        #endregion Свойства

        #region Вспомогательные функции

        protected void LoadSettings(Assembly assembly)
        {
            string settingFile = Path.Combine(WorkDir, "settings.json");

            // Копирование настроек по умолчанию
            if (!File.Exists(settingFile)) {
                string resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(str => str.EndsWith("settings.json"));

                if (string.IsNullOrEmpty(resourceName))
                    return;

                using FileStream fs = File.Create(settingFile);
                assembly
                    .GetManifestResourceStream(resourceName)
                    ?.CopyTo(fs);
            }

            Settings = JsonConvert.DeserializeObject<T>(File.ReadAllText(settingFile));
        }

        protected void Init(Assembly assembly)
        {
            WorkDir = Path.GetDirectoryName(assembly.Location);

            LoadSettings(assembly);
        }

        #endregion Вспомогательные функции

        #region Основные команды

        /// <summary>
        /// Обработка команд
        /// </summary>
        /// <returns></returns>
        public virtual BotMessage ProcessCommand(string user, string command, string data)
        {
            return new() {
                Type = BotMessageType.NotCommand
            };
        }

        /// <summary>
        /// Очистка ресурсов перед выгрузкой
        /// </summary>
        public virtual void Unload()
        {
        }

        public BotPlugin(Assembly assembly, Broadcast broadcast)
        {
            Init(assembly);
        }

        #endregion Основные команды
    }
}
