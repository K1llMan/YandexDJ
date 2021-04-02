using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Yandex.Dj.Extensions;
using Yandex.Dj.Services.Rocksmith;

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

        #endregion События

        #region Свойства

        /// <summary>
        /// Флаг логирования сообщений
        /// </summary>
        public bool LogMessages { get; private set; }

        /// <summary>
        /// Таймаут звука
        /// </summary>
        public int SoundTimeout { get; private set; }

        /// <summary>
        /// Сервис Rocksmith
        /// </summary>
        public RocksmithService Rocksmith { get; private set; }

        #endregion Свойства

        #region Вспомогательные функции

        private void Init()
        {
            // Загрузка текстовых команд
            commandList = new List<string>();

            JObject settings = JsonCommon.Load(Path.Combine(botDir.FullName, "bot.json"));
            if (!settings.IsNullOrEmpty()) {
                LogMessages = Convert.ToBoolean(settings["logMessages"]);
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
            message = message.Trim();
            if (LogMessages)
                Console.WriteLine($"{user}: {message}");

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
                        string result = Rocksmith.AddTrack(data, user);

                        return new BotMessage {
                            Type = BotMessageType.Success,
                            Text = result
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

        /// <summary>
        /// Тестирование команд бота
        /// </summary>
        public BotMessage ChatCommandTest(string user, string message)
        {
            return ProcessCommand(user, message);
        }

        public BotService(RocksmithService rocksmith)
        {
            botDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bot"));
            speechDir = new DirectoryInfo(Path.Combine(botDir.FullName, "speech"));
            speechDirCache = new DirectoryInfo(Path.Combine(speechDir.FullName, "cache"));

            if (speechDirCache.Exists)
            {
                speechDirCache.Delete(true);
                speechDirCache.Refresh();
            }

            Rocksmith = rocksmith;

            Init();
        }

        #endregion Основные функции
    }
}
