using System.IO;
using System.Reflection;

using Yandex.Dj.Bot;
using Yandex.Dj.Bot.Plugins;
using Yandex.Dj.CommonServices.WebSocket;

namespace HelloPlugin
{
    public class HelloPlugin : BotPlugin<BotPluginSettings>, IBotPlugin
    {
        #region Основные функции

        public override BotMessage ProcessCommand(string user, string command, string data)
        {
            switch (command)
            {
                case "!привет":
                    return new BotMessage
                    {
                        Text = $"Привет, {user}!",
                        Type = BotMessageType.Success
                    };
            }

            return base.ProcessCommand(user, command, data);
        }

        public HelloPlugin(Broadcast broadcast) : base(Assembly.GetExecutingAssembly(), broadcast)
        {
        }

        #endregion Основные функции
    }
}
