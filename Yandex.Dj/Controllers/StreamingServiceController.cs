using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services;
using Yandex.Dj.Services.Bot;
using Yandex.Dj.Services.ContentProviders.Common;
using Yandex.Dj.Services.Rocksmith;
using Yandex.Dj.Services.Widgets;

namespace Yandex.Dj.Controllers
{
    [Route("api/[controller]")]
    public class StreamingServiceController : Controller
    {
        #region Поля

        private StreamingService streamingService;

        #endregion Поля

        #region Вспомогательные функции

        private JObject GetBodyObject()
        {
            string data;
            using (StreamReader sr = new(Request.Body)) {
                data = sr.ReadToEndAsync().Result;
            }

            return JObject.Parse(data);
        }

        #endregion Вспомогательные функции

        #region Основные функции

        [HttpGet("playlists")]
        [Description("Получение списка плейлистов")]
        public object Playlists()
        {
            return streamingService.GetPlaylists();
        }

        [HttpGet("playlists/update")]
        [Description("Обновление плейлистов")]
        public void UpdatePlaylists(ProviderType type)
        {
            streamingService.UpdatePlaylists(type);
        }

        [HttpGet("playlist")]
        [Description("Получение данных плейлиста")]
        public object Playlist(string user, ProviderType type, string id)
        {
            return streamingService.GetPlaylist(type, id);
        }

        [HttpGet("track")]
        [Description("Получение ссылки на трек")]
        public object Track(string user, ProviderType type, string id)
        {
            return streamingService.GetMusicLink(type, id);
        }

        [HttpPost("currentSong")]
        [Description("Обновление текущей композиции")]
        public void UpdateCurrentSong(string user)
        {
            JObject data = GetBodyObject();
            if (data.IsNullOrEmpty())
                return;

            streamingService.UpdateCurrentSong(data["name"].ToString());
        }

        [HttpPost("chatTest")]
        [Description("Тестирование сообщений в чате")]
        public BotMessage ChatTest()
        {
            JObject data = GetBodyObject();
            if (data.IsNullOrEmpty())
                return new BotMessage {
                    Type = BotMessageType.Error,
                    Text = "Ошибка разбора тела сообщения"
                };

            return streamingService.Bot.ChatCommandTest(data["user"].ToString(), data["message"].ToString());
        }

        [HttpGet("tracks")]
        [Description("Список треков в очереди")]
        public List<RocksmithTrack> GetTrackList()
        {
            return streamingService.Rocksmith.TrackList;
        }

        [HttpPost("removeTrack")]
        [Description("Удаление трека из очереди")]
        public NoContentResult RemoveTrack([FromBody]RocksmithTrack track)
        {
            streamingService.Rocksmith.RemoveTrack(track);

            return NoContent();
        }

        [HttpGet("schemes")]
        [Description("Получение списка схем виджетов")]
        public object GetWidgetsSchemes()
        {
            return streamingService.WidgetsSchemes;
        } 

        [HttpGet("scheme")]
        [Description("Получение схемы виджетов")]
        public object GetWidgetsScheme()
        {
            return streamingService.WidgetsScheme.Widgets;
        }

        [HttpPost("scheme")]
        [Description("Установка схемы виджетов")]
        public void SetWidgetsScheme([FromBody]WidgetsScheme scheme)
        {
            streamingService.WidgetsScheme = scheme;
        }

        public StreamingServiceController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion Основные функции
    }
}
