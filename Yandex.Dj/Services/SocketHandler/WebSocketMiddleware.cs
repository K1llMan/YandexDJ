using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace Yandex.Dj.Services.SocketHandler
{
    /// <summary>
    /// Класс для обновления данных на всех подключённых клиентах
    /// </summary>
    public class WebSocketMiddleware
    {
        #region Поля

        private static Regex pathRegex = new Regex("/api/ws");

        private readonly RequestDelegate nextDelegate;

        #endregion Поля

        public WebSocketMiddleware(RequestDelegate next)
        {
            nextDelegate = next;
        }

        public async Task Invoke(HttpContext context, StreamingService streaming)
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

            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            WebSocketWrapper socket = streaming.Broadcast.Add(webSocket);

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
