using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Yandex.Dj.Services.SocketHandler
{
    /// <summary>
    /// Класс для обновления данных на всех подключённых клиентах
    /// </summary>
    public class WebSocketWrapper
    {
        #region Поля

        private WebSocket curSocket;

        // Сообщение
        private StringBuilder data;
        private int size;

        #endregion Поля

        #region События

        // Событие получения сообщения
        public class ReceiveEventArgs
        {
            public string Data { get; internal set; }
        }

        public delegate void RecieveEventHandler(object sender, ReceiveEventArgs e);
        public event RecieveEventHandler OnReceive;

        // Событие закрытия соединения
        public class CloseEventArgs
        {
            public WebSocketCloseStatus? Status { get; internal set; }

            public string Description { get; internal set; }
        }

        public delegate void CloseEventHandler(object sender, CloseEventArgs e);
        public event CloseEventHandler OnClose;

        // Событие закрытия соединения
        public class SendEventArgs
        {
            public string Data { get; internal set; }
        }

        public delegate void SendEventHandler(object sender, SendEventArgs e);
        public event SendEventHandler OnSend;

        #endregion События

        #region Свойства

        public string GUID { get; }

        public WebSocketState State
        {
            get { return curSocket?.State ?? WebSocketState.None; }
        }

        #endregion Свойства

        #region Основные функции

        /// <summary>
        /// Получение сообщения
        /// </summary>
        public async Task Receive()
        {
            if (curSocket.State != WebSocketState.Open)
                return;
        
            var buffer = new byte[size];
            WebSocketReceiveResult result;
            do
            {
                result = await curSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                data.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                // Вызов события 
                if (result.EndOfMessage)
                {
                    OnReceive?.Invoke(this, new ReceiveEventArgs {
                        Data = HttpUtility.UrlDecode(data.ToString())
                    });

                    data.Clear();
                }
            } while (!result.CloseStatus.HasValue);

            await curSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            OnClose?.Invoke(this, new CloseEventArgs {
                Status = curSocket.CloseStatus,
                Description = curSocket.CloseStatusDescription
            });
        }

        /// <summary>
        /// Отправка сообщения
        /// </summary>
        public async Task Send(string sendingData)
        {
            if (curSocket.State != WebSocketState.Open)
                return;

            try
            {
                byte[] encoded = Encoding.UTF8.GetBytes(sendingData);
                await curSocket.SendAsync(new ArraySegment<byte>(encoded, 0, encoded.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                OnSend?.Invoke(this, new SendEventArgs {
                    Data = data.ToString()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public async Task Close()
        {
            if (curSocket.State != WebSocketState.Open)
                return;

            await curSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        /// <summary>
        /// Добавление нового подключения
        /// </summary>
        public WebSocketWrapper(WebSocket socket, int bufferSize = 4096)
        {
            curSocket = socket;
            GUID = Guid.NewGuid().ToString();

            data = new StringBuilder();
            size = bufferSize;

            Send("{ \"name\":\"ololo\"}");
        }

        #endregion Основные функции
    }
}
