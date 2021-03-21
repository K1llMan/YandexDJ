using System.ComponentModel;
using System.IO;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services;
using Yandex.Dj.Services.ContentProviders.Common;

namespace Yandex.Dj.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : Controller
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

        [HttpGet("playlistCover")]
        [Description("Получение обложки локального плейлиста")]
        public object GetPlaylistCover(string id)
        {
            return File(streamingService.GetPlaylistCover(id), "image/*");
        }

        [HttpGet("trackCover")]
        [Description("Получение обложки локального трека")]
        public object GetTrackCover(string id)
        {
            return File(streamingService.GetTrackCover(id), "image/*");
        }

        [HttpGet("track")]
        [Description("Получение трека")]
        public object Track(ProviderType type, string id)
        {
            return File(streamingService.GetTrackContent(type, id), "audio/mpeg");
        }

        [HttpGet("speech")]
        [Description("Получение речи из текста")]
        public object Speech(string id)
        {
            return File(streamingService.Bot.GetSpeechFile(id), "audio/wav");
        }

        [HttpGet("sound")]
        [Description("Получение звука")]
        public object Sound(string id)
        {
            return File(streamingService.Bot.GetSoundFile(id), "audio/mpeg");
        }

        public ContentController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion Основные функции
    }
}
