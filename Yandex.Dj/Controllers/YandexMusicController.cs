using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services;
using Yandex.Music.Api.Common.YPlaylist;
using Yandex.Music.Api.Common.YTrack;

namespace Yandex.Dj.Controllers
{
    [Route("api/[controller]")]
    public class YandexMusicController : Controller
    {
        #region Поля

        private YandexMusicUser testUser;
        private YandexMusicService music;

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

        private string GetCover(string url, int size)
        {
            return string.IsNullOrEmpty(url) 
                ? string.Empty
                : $"http://{url.Replace("%%", $"{size}x{size}")}";
        }

        private object GetTrack(YTrack track)
        {
            return new Dictionary<string, object> {
                { "name", track.Title },
                { "singer", track.Artists.Count > 0 ? track.Artists.First().Name : string.Empty },
                { "cover", GetCover(track.CoverUri, 100) },
                { "key", track.GetKey().ToString() }
            };
        }

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
            return testUser.Playlists
                .Select(p => new Dictionary<string, object> {
                    { "title", p.Title },
                    { "cover", GetCover(p.OgImage, 100) },
                    { "tracks", p.Tracks.Select(t => GetTrack(t.Track)) }
                });
        }

        [HttpGet("playlist")]
        public object Playlist(string user, string kind)
        {
            return testUser.Playlists.FirstOrDefault(p => p.Kind == kind);
        }

        [HttpGet("track")]
        public object Track(string user, string key)
        {
            return music.GetMusicLink(testUser, key);
        }

        [HttpPost("currentSong")]
        public void UpdateCurrentSong(string user, [FromBody]JObject data)
        {
            testUser.UpdateCurrentSong(data["name"].ToString());
        }

        public YandexMusicController(YandexMusicService yandexMusic)
        {
            music = yandexMusic;
            testUser = music.Users["k1llman"];
        }

        #endregion Основные функции
    }
}
