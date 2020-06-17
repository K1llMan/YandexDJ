using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services;
using Yandex.Dj.Services.ContentProviders.Common;

namespace Yandex.Dj.Controllers
{
    [Route("api/[controller]")]
    public class StreamingServiceController : Controller
    {
        #region Поля

        private StreamingService streamingService;

        #endregion Поля

        /*
        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts(int startDateIndex)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
        */

        #region Вспомогательные функции

        #endregion Вспомогательные функции

        #region Основные функции

        [HttpGet("auth")]
        public object Auth(string login, string password)
        {
            return null;
        }

        [HttpGet("playlists")]
        public object Playlists()
        {
            return streamingService.GetPlaylists();
        }

        [HttpGet("playlist")]
        public object Playlist(string user, ProviderType type, string id)
        {
            return streamingService.GetPlaylist(type, id);
        }

        [HttpGet("track")]
        public object Track(string user, ProviderType type, string id)
        {
            return streamingService.GetMusicLink(type, id);
        }

        [HttpPost("currentSong")]
        public void UpdateCurrentSong(string user, [FromBody]JObject data)
        {
            streamingService.UpdateCurrentSong(data["name"].ToString());
        }

        public StreamingServiceController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion Основные функции
    }
}
