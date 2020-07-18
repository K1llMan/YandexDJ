using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TwitchLib.Client.Enums;
using TwitchLib.Client.Models;

using Yandex.Dj.Extensions;

namespace Yandex.Dj.Services.Twitch
{
    public class TwitchConnectorBot
    {
        #region Поля

        private TwitchConnector twitch;
        private DirectoryInfo botDir;
        private DirectoryInfo speechDir;
        private DirectoryInfo speechDirCache;

        private List<string> commandList;
        private Dictionary<string, string> soundCommandList;

        private DateTime lastSound = DateTime.Now;

        #endregion Поля

        #region События

        // Событие преобразования текста в речь
        public class TextToSpeechEventArgs
        {
            public string FileName { get; internal set; }
        }

        public delegate void TextToSpeechHandler(TextToSpeechEventArgs e);
        public event TextToSpeechHandler TextToSpeechEvent;

        // Событие голосового сообщения
        public class SoundMessageEventArgs
        {
            public string FileName { get; internal set; }
        }

        public delegate void SoundMessageHandler(SoundMessageEventArgs e);
        public event SoundMessageHandler SoundMessageEvent;

        #endregion События

        #region Свойства

        public int SoundTimeout { get; private set; }

        #endregion Свойства

        #region Вспомогательные функции

        /// <summary>
        /// Отправить сообщение в чат
        /// </summary>
        private void Send(string message)
        {
            twitch.Send(message);
        }

        private void Init()
        {
            // Загрузка текстовых команд
            commandList = new List<string>();

            JObject settings = JsonCommon.Load(Path.Combine(botDir.FullName, "bot.json"));
            if (!settings.IsNullOrEmpty()) {
                commandList = JsonConvert.DeserializeObject<List<string>>(settings["commands"].ToString());
                SoundTimeout = Convert.ToInt32(settings["soundTimeout"].ToString());
            }

            // Загрузка звуковых команд
            soundCommandList = JsonCommon.Load<Dictionary<string, string>>(Path.Combine(botDir.FullName, "sound.json"));
        }

        private string TextToSpeech(string text)
        {
            if (!speechDirCache.Exists) {
                speechDirCache.Create();
                speechDirCache.Refresh();
            }

            string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string path = Path.Combine(speechDirCache.FullName, $"{fileName}.wav");

            Process process = new Process {
                StartInfo = new ProcessStartInfo(Path.Combine(speechDir.FullName, "balcon.exe"), 
                    $"-t \"{text}\" -w \"{path}\""
                )
            };

            process.Start();
            process.WaitForExit();

            return fileName;
        }

        #endregion Вспомогательные функции

        #region Обработка команд

        public bool ProcessCommand(ChatMessage chatMessage)
        {
            string message = chatMessage.Message.Trim();

            if (message.StartsWith("!")) {
                string command = message.GetMatches(@"^!\w+").First();
                string data = message.GetMatches(@"(?<=\b[\s]).+").FirstOrDefault();

                if (!commandList.Contains(command) && !soundCommandList.ContainsKey(command.TrimStart('!')))
                    return false;

                switch (command) {
                    case "!привет":
                        Send($"Привет, {chatMessage.DisplayName}!");
                        break;
                    case "!озвучить":
                        if (DateTime.Now < lastSound.AddSeconds(SoundTimeout))
                            return true;

                        string id = TextToSpeech(data);
                        TextToSpeechEvent?.Invoke(new TextToSpeechEventArgs {
                            FileName = id
                        });

                        lastSound = DateTime.Now;
                        break;
                    case "!time":
                        if (!twitch.API.V5.Streams.BroadcasterOnlineAsync(twitch.UserID).GetAwaiter().GetResult()) {
                            Send($"Трансляция не ведётся.");
                            break;
                        }

                        TimeSpan? time = twitch.API.V5.Streams.GetUptimeAsync(twitch.UserID).GetAwaiter().GetResult();
                        Send($"Время в эфире: {time}");
                        break;
                    default:
                        if (soundCommandList.ContainsKey(command.TrimStart('!'))) {
                            if (DateTime.Now < lastSound.AddSeconds(SoundTimeout))
                                return true;

                            SoundMessageEvent?.Invoke(new SoundMessageEventArgs {
                                FileName = command.TrimStart('!')
                            });

                            lastSound = DateTime.Now;
                            break;
                        }
                        break;
                }

                return true;
            }

            return false;
        }

        #endregion Обработка команд

        #region Основные функции

        public FileStream GetSpeechFile(string id)
        {
            FileInfo file = speechDirCache.GetFiles($"{id}.wav").FirstOrDefault();
            if (file == null)
                return null;

            return new FileStream(file.FullName, FileMode.Open);
        }

        public FileStream GetSoundFile(string id)
        {
            if (!soundCommandList.ContainsKey(id))
                return null;

            return new FileStream(soundCommandList[id], FileMode.Open);
        }

        public void ChatCommandTest(string message)
        {
            ProcessCommand(new ChatMessage(
                "test", "test", "test", "Test", "#fff", Color.White,
                null, message, UserType.Viewer, twitch.Channel, "test", false, -1, "test",
                false, false, false, false, Noisy.NotSet,
                message, message, null, null, 0, 0));
        }

        public TwitchConnectorBot(TwitchConnector connector)
        {
            twitch = connector;
            botDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bot"));
            speechDir = new DirectoryInfo(Path.Combine(botDir.FullName, "speech"));
            speechDirCache = new DirectoryInfo(Path.Combine(speechDir.FullName, "cache"));

            if (speechDirCache.Exists) {
                speechDirCache.Delete(true);
                speechDirCache.Refresh();
            }

            Init();
        }

        #endregion Основные функции
    }
}
