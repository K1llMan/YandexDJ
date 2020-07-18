using System;
using System.Linq;

using Newtonsoft.Json.Linq;

using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.V5.Models.Users;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Yandex.Dj.Services.Twitch
{
    public class TwitchConnector
    {
        #region Поля

        private TwitchClient client;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Бот
        /// </summary>
        public TwitchConnectorBot Bot { get; private set; }

        /// <summary>
        /// API
        /// </summary>
        public TwitchAPI API { get; private set; }

        /// <summary>
        /// Канал
        /// </summary>
        public string Channel { get; private set; }

        /// <summary>
        /// Идентификатор пользователя для API
        /// </summary>
        public string UserID { get; private set; }

        #endregion Свойства

        #region Вспомогательные функции

        private void Client_OnLog(object sender, OnLogArgs args)
        {
            Console.WriteLine(args.Data);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs args)
        {
            Console.WriteLine(args.BotUsername);
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs args)
        {
            if (!Bot.ProcessCommand(args.ChatMessage))
                Console.WriteLine(args.ChatMessage.Message);
        }

        private void ClientConnect(JObject config)
        {
            // Token сгенерирован twitchtokengenerator
            ConnectionCredentials credentials = new ConnectionCredentials(config["login"].ToString(), config["token"].ToString());
            ClientOptions clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, Channel);

            //client.OnLog += Client_OnLog;
            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += OnMessageReceived;

            /*
            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            */

            client.Connect();
        }

        private void APIConnect(JObject config)
        {
            API = new TwitchAPI(settings: new ApiSettings
            {
                ClientId = config["clientId"].ToString(),
                AccessToken = config["token"].ToString()
            });


            User[] users = API.V5.Users.GetUserByNameAsync(Channel).GetAwaiter().GetResult().Matches;
            if (users.Length > 0)
                UserID = users.First().Id;
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public void Send(string message)
        {
            client.SendMessage(Channel, message);
        }

        public TwitchConnector(JObject config)
        {
            Bot = new TwitchConnectorBot(this);
            Channel = config["channel"].ToString();

            ClientConnect(config);
            APIConnect(config);
        }

        #endregion Основные функции
    }
}
