using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using RocksmithPlugin.Classes;

using Yandex.Dj.Bot;
using Yandex.Dj.Bot.Plugins;
using Yandex.Dj.CommonServices.WebSocket;

namespace RocksmithPlugin
{
    public class RocksmithPlugin : BotPlugin<RocksmithSettings>, IBotPlugin
    {
        #region Свойства

        /// <summary>
        /// Очередь треков
        /// </summary>
        public RocksmithQueue Queue { get; }

        #endregion Свойства

        #region Вспомогательные функции

        /// <summary>
        /// Добавление трека
        /// </summary>
        private string AddTrack(string data, string user)
        {
            List<string> parts = data.Split(Settings.Separators, StringSplitOptions.RemoveEmptyEntries)
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
        private void RemoveTrack(RocksmithTrack track)
        {
            Queue.RemoveTrack(track);
        }

        /// <summary>
        /// События для веб-сокетов
        /// </summary>
        /// <param name="broadcast"></param>
        private void InitBroadcastEvents(Broadcast broadcast)
        {
            // События Rocksmith
            Queue.TrackAddEvent += async eventArgs => {
                await broadcast.Send(new BroadcastEvent {
                    Event = "addRocksmithTrack",
                    Data = eventArgs.Track
                });
            };

            Queue.TrackRemoveEvent += async eventArgs => {
                await broadcast.Send(new BroadcastEvent {
                    Event = "removeRocksmithTrack",
                    Data = eventArgs.Track
                });
            };

            broadcast.On("getRocksmithTracks", async (wrapper, o) => {
                await wrapper.Send(Broadcast.SerializeMessage(new BroadcastEvent {
                    Event = "updateRocksmithTracks",
                    Data = Queue.Tracks
                }));
            });
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public override BotMessage ProcessCommand(string user, string command, string data)
        {
            switch (command)
            {
                case "!sr":
                case "!заказ":
                    string result = AddTrack(data, user);

                    return new BotMessage {
                        Type = BotMessageType.Success,
                        Text = result
                    };
            }

            return base.ProcessCommand(user, command, data);
        }

        public RocksmithPlugin(Broadcast broadcast) : base(Assembly.GetExecutingAssembly(), broadcast)
        {
            Queue = new RocksmithQueue(WorkDir, Settings.UserLimit);
            InitBroadcastEvents(broadcast);
        }

        #endregion Основные функции
    }
}
