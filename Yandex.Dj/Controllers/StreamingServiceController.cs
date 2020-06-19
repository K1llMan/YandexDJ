using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services;
using Yandex.Dj.Services.ContentProviders.Common;

namespace Yandex.Dj.Controllers
{
    [Route("api/[controller]")]
    public class StreamingServiceController : Controller
    {
        #region ����

        private StreamingService streamingService;

        #endregion ����

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

        #region ��������������� �������

        private JObject GetBodyObject()
        {
            string data;
            using (StreamReader sr = new StreamReader(Request.Body)) {
                data = sr.ReadToEndAsync().Result;
            }

            return JObject.Parse(data);
        }

        #endregion ��������������� �������

        #region �������� �������

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

        [HttpGet("playlists/update")]
        public void UpdatePlaylists(ProviderType type)
        {
            streamingService.UpdatePlaylists(type);
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
        public void UpdateCurrentSong(string user)
        {
            JObject data = GetBodyObject();
            if (data.IsNullOrEmpty())
                return;

            streamingService.UpdateCurrentSong(data["name"].ToString());
        }

        public StreamingServiceController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion �������� �������
    }
}
