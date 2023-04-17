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

using Yandex.Dj.Bot;

namespace Yandex.Dj.Services.Streaming.Twitch
{
    public class TwitchConnector
    {
        #region Поля

        private TwitchClient client;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Конфигурация
        /// </summary>
        internal TwitchConnectorConfig Config { get; }

        /// <summary>
        /// Бот
        /// </summary>
        public BotService Bot { get; private set; }

        /// <summary>
        /// API
        /// </summary>
        public TwitchAPI API { get; private set; }

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
            BotMessage message = Bot.ProcessCommand(args.ChatMessage.Username, args.ChatMessage.Message);

            switch(message) {
                case { Type: BotMessageType.Error }:  
                    Console.WriteLine(args.ChatMessage.Message);
                    break;

                case { Type: BotMessageType.Success } when !string.IsNullOrEmpty(message.Text) :
                    Send(message.Text);
                    break;
            };
        }

        private void ClientConnect()
        {
            // Token сгенерирован twitchtokengenerator
            ConnectionCredentials credentials = new(Config.Login, Config.Token);
            ClientOptions clientOptions = new() {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, Config.Channel);

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

        private void APIConnect()
        {
            API = new TwitchAPI(settings: new ApiSettings {
                ClientId = Config.ClientId,
                AccessToken = Config.Token
            });


            User[] users = API.V5.Users.GetUserByNameAsync(Config.Channel).GetAwaiter().GetResult().Matches;
            if (users.Length > 0)
                UserID = users.First().Id;
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public void Send(string message)
        {
            client.SendMessage(Config.Channel, message);
        }

        public TwitchConnector(BotService bot, TwitchConnectorConfig config)
        {
            Bot = bot;
            Config = config;

            ClientConnect();
            APIConnect();
        }

        #endregion Основные функции
    }
}
