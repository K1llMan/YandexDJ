using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using Yandex.Dj.Services.SocketHandler;

namespace Yandex.Dj.Services
{
    /// <summary>
    /// Событие рассылки
    /// </summary>
    public class BroadcastEvent
    {
        public string Event;

        public object Data;
    }

    /// <summary>
    /// Класс для обновления данных на всех подключённых клиентах
    /// </summary>
    public class Broadcast
    {
        #region Поля

        private ConcurrentDictionary<string, WebSocketWrapper> sockets = new ConcurrentDictionary<string, WebSocketWrapper>();
        private Dictionary<string, List<Action<WebSocketWrapper,object>>> handlers = new Dictionary<string, List<Action<WebSocketWrapper,object>>>();

        #endregion Поля

        #region Основные функции

        #region Подписка на события

        public void On(string eventName, Action<WebSocketWrapper,object> handler)
        {
            if (!handlers.ContainsKey(eventName))
                handlers.Add(eventName, new List<Action<WebSocketWrapper,object>>());

            if (!handlers[eventName].Contains(handler))
                handlers[eventName].Add(handler);
        }

        #endregion Подписка на события

        /// <summary>
        /// Рассылка данных всем клиентам
        /// </summary>
        public async Task Send(params BroadcastEvent[] actions)
        {
            foreach (BroadcastEvent action in actions)
            {
                string dataStr = JsonConvert.SerializeObject(action, Formatting.None, new JsonSerializerSettings{ ContractResolver = new CamelCasePropertyNamesContractResolver() });

                foreach (WebSocketWrapper socket in sockets.Values)
                    await socket.Send(dataStr);
            }
        }

        /// <summary>
        /// Исполнение команды
        /// </summary>
        public async Task Invoke(WebSocketWrapper sender, string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr))
                return;

            try {
                BroadcastEvent broadcastEvent = JsonConvert.DeserializeObject<BroadcastEvent>(dataStr);
                if (handlers.ContainsKey(broadcastEvent.Event))
                    foreach (Action<WebSocketWrapper, object> action in handlers[broadcastEvent.Event])
                        action(sender, broadcastEvent.Data);
            }
            catch (Exception ex) {
            }

        }

        /// <summary>
        /// Добавление нового подключения
        /// </summary>
        public WebSocketWrapper Add(WebSocket socket)
        {
            WebSocketWrapper webSocketWrapper = new WebSocketWrapper(socket);

            webSocketWrapper.OnReceive += async (s, args) => await Invoke(s, args.Data);
            webSocketWrapper.OnClose += (s, args) => {
                Remove(((WebSocketWrapper)s).GUID);
            };

            sockets.TryAdd(webSocketWrapper.GUID, webSocketWrapper);
            return webSocketWrapper;
        }

        /// <summary>
        /// Удаление подключения
        /// </summary>
        public void Remove(string guid)
        {
            WebSocketWrapper socket = null;
            if (sockets.ContainsKey(guid))
                sockets.TryRemove(guid, out socket);

            socket?.Close();
        }

        /// <summary>
        /// Закрытие всех сокетов
        /// </summary>
        public async void Stop()
        {
            foreach (WebSocketWrapper socket in sockets.Values)
                await socket.Close();
        }

        #endregion Основные функции
    }
}
