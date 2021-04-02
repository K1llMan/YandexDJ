using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services.Bot;

namespace Yandex.Dj.Services.Streaming
{
    public class YoutubeConnector: IDisposable
    {
        #region Поля

        private YouTubeService youtube;
        private CancellationTokenSource cancellationSource = new();
        private CancellationToken cancellationToken;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Бот
        /// </summary>
        public BotService Bot { get; }

        /// <summary>
        /// Идентификатор канала
        /// </summary>
        public string ChannelId { get; private set; }

        /// <summary>
        /// Идентификатор чата
        /// </summary>
        public string ChatId { get; set; }

        #endregion Свойства

        #region Вспомогательные функции

        private async Task<string> GetChatPage(string nextPageToken)
        {
            if (string.IsNullOrEmpty(ChatId))
                return null;

            LiveChatMessagesResource.ListRequest chatRequest = youtube.LiveChatMessages.List(ChatId, "snippet,authorDetails");
            chatRequest.PageToken = nextPageToken;
            LiveChatMessageListResponse chatResponse = chatRequest.Execute();

            // Обработка всех сообщений со страницы
            foreach (LiveChatMessage message in chatResponse.Items)
                Bot.ProcessCommand(message.AuthorDetails.DisplayName, message.Snippet.DisplayMessage);

            await Task.Delay((int) chatResponse.PollingIntervalMillis);

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
            searchRequest.ChannelId = ChannelId; //"UCyEaMDuqyql3n3e_qGEiE9w";
            searchRequest.EventType = SearchResource.ListRequest.EventTypeEnum.Live;
            searchRequest.Type = "video";

            SearchListResponse searchResponse = searchRequest.Execute();
            string videoId = searchResponse.Items.FirstOrDefault()?.Id.VideoId;

            VideosResource.ListRequest videoRequest = youtube.Videos.List("liveStreamingDetails");
            videoRequest.Id = videoId;

            VideoListResponse videoResponse = videoRequest.Execute();
            return videoResponse.Items.FirstOrDefault()?.LiveStreamingDetails.ActiveLiveChatId;
        }

        private void InitClient(JObject config)
        {
            youtube = new(new BaseClientService.Initializer {
                ApiKey = config["apiKey"].ToString(),
                ApplicationName = GetType().ToString()
            });

            ChannelId = config["channelId"].ToString();
            ChatId = GetChatId();
        }

        #endregion Вспомогательные функции

        #region Основные функции

        public YoutubeConnector(BotService bot, JObject config)
        {
            Bot = bot;

            InitClient(config);

            cancellationToken = cancellationSource.Token;
            StartChatListener(cancellationSource.Token);
        }

        #endregion Основные функции

        public void Dispose()
        {
            cancellationSource.Cancel(false);
            youtube?.Dispose();
        }
    }
}
