using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

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
        /// Канал
        /// </summary>
        public string Channel { get; private set; }

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

        private void Connect(JObject config)
        {
            Channel = config["channel"].ToString();

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

        #endregion Вспомогательные функции

        #region Основные функции

        public void Send(string message)
        {
            client.SendMessage(Channel, message);
        }

        public TwitchConnector(JObject config)
        {
            Bot = new TwitchConnectorBot(this);

            Connect(config);
        }

        #endregion Основные функции
    }
}
