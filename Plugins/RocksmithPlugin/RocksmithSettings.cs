using Yandex.Dj.Bot.Plugins;

namespace RocksmithPlugin
{
    public class RocksmithSettings: BotPluginSettings
    {
        #region Свойства

        public string[] Separators { get; set; }
        public int UserLimit { get; set; }

        #endregion Свойства
    }
}
