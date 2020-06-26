using System.ComponentModel;
using System.IO;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using Yandex.Dj.Services;

namespace Yandex.Dj.Controllers
{
    [Route("api/[controller]")]
    public class ContentController : Controller
    {
        #region ����

        private StreamingService streamingService;

        #endregion ����

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

        [HttpGet("playlistCover")]
        [Description("��������� ������� ���������� ���������")]
        public object GetPlaylistCover(string id)
        {
            return File(streamingService.GetPlaylistCover(id), "image/*");
        }

        [HttpGet("trackCover")]
        [Description("��������� ������� ���������� �����")]
        public object GetTrackCover(string id)
        {
            return File(streamingService.GetTrackCover(id), "image/*");
        }

        [HttpGet("track")]
        [Description("��������� ���������� �����")]
        public object Track(string id)
        {
            return File(streamingService.GetTrackContent(id), "audio/mpeg");
        }

        [HttpGet("speech")]
        [Description("��������� ���� �� ������")]
        public object Speech(string id)
        {
            return File(streamingService.Twitch.Bot.GetSpeechFile(id), "audio/wav");
        }

        [HttpGet("sound")]
        [Description("��������� �����")]
        public object Sound(string id)
        {
            return File(streamingService.Twitch.Bot.GetSoundFile(id), "audio/mpeg");
        }

        public ContentController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion �������� �������
    }
}
