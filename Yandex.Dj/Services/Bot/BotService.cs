using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;

namespace Yandex.Dj.Services.Bot
{
    public class BotService
    {
        #region Поля

        // Директории бота
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

        // Событие добавления/удаления трека
        public class TrackEventArgs {
            public RocksmithTrack Track { get; internal set; }
        }

        public delegate void TrackHandler(TrackEventArgs e);
        public event TrackHandler TrackAddEvent;

        public event TrackHandler TrackRemoveEvent;

        #endregion События

        #region Свойства

        /// <summary>
        /// Таймаут звука
        /// </summary>
        public int SoundTimeout { get; private set; }

        /// <summary>
        /// Список треков
        /// </summary>
        public List<RocksmithTrack> TrackList { get; private set; }

        #endregion Свойства

        #region Вспомогательные функции

        private void Init()
        {
            // Загрузка текстовых команд
            commandList = new List<string>();

            JObject settings = JsonCommon.Load(Path.Combine(botDir.FullName, "bot.json"));
            if (!settings.IsNullOrEmpty())
            {
                commandList = JsonConvert.DeserializeObject<List<string>>(settings["commands"].ToString());
                SoundTimeout = Convert.ToInt32(settings["soundTimeout"].ToString());
            }

            // Загрузка звуковых команд
            soundCommandList = JsonCommon.Load<Dictionary<string, string>>(Path.Combine(botDir.FullName, "sound.json"));
        }

        private string TextToSpeech(string text)
        {
            if (!speechDirCache.Exists)
            {
                speechDirCache.Create();
                speechDirCache.Refresh();
            }

            string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string path = Path.Combine(speechDirCache.FullName, $"{fileName}.wav");

            Process process = new() {
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

        public BotMessage ProcessCommand(string user, string message)
        {
            if (message.StartsWith("!"))
            {
                string command = message.GetMatches(@"^!\w+").First();
                string data = message.GetMatches(@"(?<=\b[\s]).+").FirstOrDefault();

                if (!commandList.Contains(command) && !soundCommandList.ContainsKey(command.TrimStart('!')))
                    return new BotMessage {
                        Type = BotMessageType.NotCommand
                    };

                switch (command)
                {
                    case "!привет":
                        return new BotMessage {
                            Text = $"Привет, {user}!",
                            Type = BotMessageType.Success
                        };

                    case "!озвучить":
                        if (DateTime.Now < lastSound.AddSeconds(SoundTimeout))
                            return new BotMessage {
                                Type = BotMessageType.Error
                            };

                        string id = TextToSpeech(data);
                        TextToSpeechEvent?.Invoke(new TextToSpeechEventArgs {
                            FileName = id
                        });

                        lastSound = DateTime.Now;
                        break;

                    case "!time":
                        return new BotMessage {
                            Text = $"Время в эфире: {DateTime.Now}",
                            Type = BotMessageType.Success
                        };

                    case "!sr":
                    case "!заказ":
                        List<string> parts = data.Split("|")
                            .Select(p => p.Trim())
                            .ToList();

                        for (int i = 0; i < 4 - parts.Count; i++) 
                            parts.Add("Any");

                        Enum.TryParse(parts[2], out RocksmithTrackArrangement arrangement);

                        RocksmithTrack track = new() {
                            Artist = parts[0],
                            Name = parts[1],
                            User = user,
                            ArrangementType = arrangement
                        };

                        TrackList.Add(track);

                        TrackAddEvent?.Invoke(new TrackEventArgs {
                            Track = track
                        });

                        return new BotMessage {
                            Type = BotMessageType.Success,
                            Text = "Трек добавлен"
                        };

                    default:
                        if (soundCommandList.ContainsKey(command.TrimStart('!')))
                        {
                            if (DateTime.Now < lastSound.AddSeconds(SoundTimeout))
                                return new BotMessage {
                                    Type = BotMessageType.Error
                                };

                            SoundMessageEvent?.Invoke(new SoundMessageEventArgs {
                                FileName = command.TrimStart('!')
                            });

                            lastSound = DateTime.Now;
                        }
                        break;
                }

                return new BotMessage {
                    Type = BotMessageType.Success
                };
            }

            return new BotMessage {
                Type = BotMessageType.NotCommand
            };
        }

        #endregion Обработка команд

        #region Основные функции

        /// <summary>
        /// Получение файла с речью
        /// </summary>
        public FileStream GetSpeechFile(string id)
        {
            FileInfo file = speechDirCache.GetFiles($"{id}.wav").FirstOrDefault();
            if (file == null)
                return null;

            return new FileStream(file.FullName, FileMode.Open);
        }

        /// <summary>
        /// Получения файла со звуком
        /// </summary>
        public FileStream GetSoundFile(string id)
        {
            if (!soundCommandList.ContainsKey(id))
                return null;

            return new FileStream(soundCommandList[id], FileMode.Open);
        }

        public void RemoveTrack(RocksmithTrack track)
        {
            if (!TrackList.Contains(track))
                return;

            TrackList.Remove(track);

            TrackRemoveEvent?.Invoke(new TrackEventArgs {
                Track = track
            });
        }

        /// <summary>
        /// Тестирование команд бота
        /// </summary>
        public BotMessage ChatCommandTest(string user, string message)
        {
            return ProcessCommand(user, message);
        }

        public BotService()
        {
            botDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bot"));
            speechDir = new DirectoryInfo(Path.Combine(botDir.FullName, "speech"));
            speechDirCache = new DirectoryInfo(Path.Combine(speechDir.FullName, "cache"));

            if (speechDirCache.Exists)
            {
                speechDirCache.Delete(true);
                speechDirCache.Refresh();
            }

            TrackList = new List<RocksmithTrack>();

            Init();
        }

        #endregion Основные функции
    }
}
