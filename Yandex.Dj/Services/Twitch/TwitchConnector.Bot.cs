using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc.Formatters;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        #endregion Поля

        #region События

        // Событие преобразования текста в речь
        public class TextToSpeechEventArgs
        {
            public string FileName { get; internal set; }
        }

        public delegate void TextToSpeechHandler(TextToSpeechEventArgs e);
        public event TextToSpeechHandler TextToSpeechEventEvent;

        #endregion События

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
            commandList = new List<string>();

            JObject settings = JsonCommon.Load(Path.Combine(botDir.FullName, "bot.json"));
            if (!settings.IsNullOrEmpty())
                commandList = JsonConvert.DeserializeObject<List<string>>(settings["commands"].ToString());
        }

        private string TextToSpeech(string text)
        {
            if (!speechDirCache.Exists) {
                speechDirCache.Create();
                speechDirCache.Refresh();
            }

            string fileName = Path.GetRandomFileName();
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
                string command = message.GetMatches(@"^!\w+").FirstOrDefault();
                string data = message.GetMatches(@"(?<=\b[\s]).+").FirstOrDefault();

                if (!commandList.Contains(command))
                    return false;

                Console.WriteLine($"Сообщение \"{message}\" обработано!");

                if (command == "!привет") 
                    Send($"Привет, {chatMessage.DisplayName}!");

                if (command == "!озвучить") {
                    string id = TextToSpeech(data);
                    TextToSpeechEventEvent?.Invoke(new TextToSpeechEventArgs {
                        FileName = id
                    });
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
