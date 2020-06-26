using System.ComponentModel;
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

        [HttpGet("playlists")]
        [Description("��������� ������ ����������")]
        public object Playlists()
        {
            return streamingService.GetPlaylists();
        }

        [HttpGet("playlists/update")]
        [Description("���������� ����������")]
        public void UpdatePlaylists(ProviderType type)
        {
            streamingService.UpdatePlaylists(type);
        }

        [HttpGet("playlist")]
        [Description("��������� ������ ���������")]
        public object Playlist(string user, ProviderType type, string id)
        {
            return streamingService.GetPlaylist(type, id);
        }

        [HttpGet("track")]
        [Description("��������� ������ �� ����")]
        public object Track(string user, ProviderType type, string id)
        {
            return streamingService.GetMusicLink(type, id);
        }

        [HttpPost("currentSong")]
        [Description("���������� ������� ����������")]
        public void UpdateCurrentSong(string user)
        {
            JObject data = GetBodyObject();
            if (data.IsNullOrEmpty())
                return;

            streamingService.UpdateCurrentSong(data["name"].ToString());
        }

        [HttpPost("chatTest")]
        [Description("������������ ��������� � ����")]
        public void ChatTest()
        {
            JObject data = GetBodyObject();
            if (data.IsNullOrEmpty())
                return;

            streamingService.Twitch.Bot.ChatCommandTest(data["message"].ToString());
        }

        [HttpGet("scheme")]
        [Description("��������� ����� ��������")]
        public object GetWidgetsScheme()
        {
            return streamingService.WidgetsScheme?.ToString();
        }

        public StreamingServiceController(StreamingService streaming)
        {
            streamingService = streaming;
        }

        #endregion �������� �������
    }
}
