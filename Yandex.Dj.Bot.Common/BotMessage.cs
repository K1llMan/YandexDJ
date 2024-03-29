﻿using System;

namespace Yandex.Dj.Bot
{
    public class BotMessage
    {
        public DateTime Time { get; set; } = DateTime.Now;

        public string Text { get; set; }

        public BotMessageType Type { get; set; }
    }
}
