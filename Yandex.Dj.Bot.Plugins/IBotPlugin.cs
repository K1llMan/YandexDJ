using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yandex.Dj.Bot.Plugins
{
    public interface IBotPlugin
    {
        BotMessage ProcessCommand(string user, string command, string data);
    }
}
