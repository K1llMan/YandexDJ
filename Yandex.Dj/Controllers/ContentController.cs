using System.IO;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services;

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
            using (StreamReader sr = new StreamReader(Request.Body)) {
                data = sr.ReadToEndAsync().Result;
            }

            return JObject.Parse(data);
        }

        #endregion Вспомогательные функции

        #region Основные функции

        [HttpGet("playlistCover")]
        public object GetPlaylistCover(string id)
        {
            return File(streamingService.GetPlaylistCover(id), "image/*");
        }

        [HttpGet("trackCover")]
        public object GetTrackCover(string id)
        {
            return File(streamingService.GetTrackCover(id), "image/*");
        }

        [HttpGet("track")]
        public object Track(string id)
        {
            return File(streamingService.GetTrackContent(id), "audio/mpeg");
        }

        [HttpGet("speech")]
        public object Speech(string id)
        {
            return File(streamingService.Twitch.Bot.GetSpeechFile(id), "audio/wav");
        }

        [HttpGet("sound")]
        public object Sound(string id)
        {
            return File(streamingService.Twitch.Bot.GetSoundFile(id), "audio/mpeg");
        }

        public ContentController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion Основные функции
    }
}
