using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Yandex.Dj.CommonServices.WebSocket
{
    /// <summary>
    /// Класс для обновления данных на всех подключённых клиентах
    /// </summary>
    public class WebSocketMiddleware
    {
        #region Поля

        private static Regex pathRegex = new("/api/ws");

        private readonly RequestDelegate nextDelegate;

        #endregion Поля

        public WebSocketMiddleware(RequestDelegate next)
        {
            nextDelegate = next;
        }

        public async Task Invoke(HttpContext context, Broadcast broadcast)
        {
            if (!pathRegex.IsMatch(context.Request.Path))
            {
                await nextDelegate(context);
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            System.Net.WebSockets.WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            WebSocketWrapper socket = broadcast.Add(webSocket);

            try
            {
                await socket.Receive();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
