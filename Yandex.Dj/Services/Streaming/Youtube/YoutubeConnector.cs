using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Yandex.Dj.Services.Bot;

namespace Yandex.Dj.Services.Streaming.Youtube
{
    public class YoutubeConnector: IDisposable
    {
        #region Поля

        private YouTubeService youtube;
        private CancellationTokenSource cancellationSource = new();

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Конфигурация
        /// </summary>
        internal YoutubeConnectorConfig Config { get; }

        /// <summary>
        /// Бот
        /// </summary>
        public BotService Bot { get; }

        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public string ChatId { get; set; }

        #endregion Свойства

        #region Вспомогательные функции

        private void Send(string message)
        {
            youtube.LiveChatMessages.Insert(new LiveChatMessage {
                Snippet = new LiveChatMessageSnippet {
                    LiveChatId = ChatId,
                    Type = "textMessageEvent",
                    TextMessageDetails = new LiveChatTextMessageDetails {
                        MessageText = message
                    }
                }
            }, "snippet");
        }

        private async Task<string> GetChatPage(string nextPageToken)
        {
            if (string.IsNullOrEmpty(ChatId))
                return string.Empty;

            LiveChatMessagesResource.ListRequest chatRequest = youtube.LiveChatMessages.List(ChatId, "snippet,authorDetails");
            chatRequest.PageToken = nextPageToken;
            LiveChatMessageListResponse chatResponse = chatRequest.Execute();

            // Обработка всех сообщений со страницы
            foreach (LiveChatMessage message in chatResponse.Items) {
                BotMessage msg = Bot.ProcessCommand(message.AuthorDetails.DisplayName, message.Snippet.DisplayMessage);

                switch (msg)
                {
                    case { Type: BotMessageType.Error }:
                        Console.WriteLine(message.Snippet.DisplayMessage);
                        break;

                    case { Type: BotMessageType.Success } when !string.IsNullOrEmpty(msg.Text):
                        //Send(msg.Text);
                        break;
                };
            }

            await Task.Delay(Math.Max(Config.Interval, (int)chatResponse.PollingIntervalMillis));

            return chatResponse.NextPageToken;
        }

        public async void StartChatListener(CancellationToken stoppingToken)
        {
            string nextPageToken = string.Empty;
            while (!stoppingToken.IsCancellationRequested) {
                nextPageToken = await GetChatPage(nextPageToken);
            }
        }

        private string GetChatId()
        {
            SearchResource.ListRequest searchRequest = youtube.Search.List("snippet");
            searchRequest.ChannelId = Config.ChannelId;
            searchRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            searchRequest.Type = "video";

            SearchListResponse searchResponse = searchRequest.Execute();

            string videoId = searchResponse.Items.FirstOrDefault()?.Id.VideoId;

            VideosResource.ListRequest videoRequest = youtube.Videos.List("liveStreamingDetails");
            videoRequest.Id = videoId;

            VideoListResponse videoResponse = videoRequest.Execute();
            return videoResponse.Items.FirstOrDefault()?.LiveStreamingDetails.ActiveLiveChatId;
        }

        private void InitClient()
        {
            youtube = new(new BaseClientService.Initializer {
                ApiKey = Config.ApiKey,
                ApplicationName = GetType().ToString()
            });

            ChatId = GetChatId();
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public YoutubeConnector(BotService bot, YoutubeConnectorConfig config)
        {
            Bot = bot;
            Config = config;

            try {
                InitClient();

                StartChatListener(cancellationSource.Token);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        #endregion Основные функции

        public void Dispose()
        {
            cancellationSource.Cancel(false);
            youtube?.Dispose();
        }
    }
}
