using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Yandex.Dj.CommonServices.WebSocket;

namespace Yandex.Dj.Bot.Plugins
{
    public class BotPluginManager
    {
        #region Поля

        private List<IBotPlugin> plugins;

        #endregion Поля

        #region Свойства


        #endregion Свойства

        #region Вспомогательные функции

        private IBotPlugin LoadPlugin(string fileName, Broadcast broadcast)
        {
            try {
                Assembly assembly = Assembly.LoadFrom(fileName);

                Type type = assembly.GetTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(IBotPlugin)));

                if (type == null)
                    return null;

                return (IBotPlugin) Activator.CreateInstance(type, broadcast);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        private void LoadPlugins(Broadcast broadcast)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            foreach (string file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories)) {
                IBotPlugin plugin = LoadPlugin(file, broadcast);

                if (plugin != null)
                    plugins.Add(plugin);
            }
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public BotMessage ProcessCommand(string user, string command, string data)
        {
            foreach (IBotPlugin plugin in plugins) {
                BotMessage msg = plugin.ProcessCommand(user, command, data);
                if (msg.Type != BotMessageType.NotCommand)
                    return msg;
            }

            return new() {
                Type = BotMessageType.NotCommand
            };
        }

        public BotPluginManager(Broadcast broadcast)
        {
            plugins = new List<IBotPlugin>();

            // Загрузка плагинов из директории
            LoadPlugins(broadcast);
        }

        #endregion Основные функции
    }
}
