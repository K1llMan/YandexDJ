using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Yandex.Dj.Services.Rocksmith
{
    public class RocksmithQueue
    {
        #region Поля

        private object saveLock;
        private string rocksmithDir;
        private int userLimit;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Список треков
        /// </summary>
        public List<RocksmithTrack> Tracks { get; set; }

        /// <summary>
        /// Список отложенных треков
        /// </summary>
        public List<RocksmithTrack> DeferredTracks { get; set; }

        #endregion Свойства

        #region События

        // Событие добавления/удаления трека
        public class TrackEventArgs {
            public RocksmithTrack Track { get; internal set; }
        }

        public delegate void TrackHandler(TrackEventArgs e);
        public event TrackHandler TrackAddEvent;

        public event TrackHandler TrackRemoveEvent;

        #endregion События

        #region Вспомогательные функции

        private void Load()
        {
            string fileName = Path.Combine(rocksmithDir, "queue.json");
            if (File.Exists(fileName))
                JsonConvert.PopulateObject(File.ReadAllText(fileName), this);
        }

        private void Save()
        {
            lock (saveLock) {
                File.WriteAllText(Path.Combine(rocksmithDir, "queue.json"), JsonSerializer.Serialize(this));
            }
        }

        #endregion Вспомогательные функции

        #region Основные функции

        /// <summary>
        /// Добавление трека
        /// </summary>
        public string Add(RocksmithTrack track)
        {
            // Если список треков от пользователя достиг лимита, то добавляем в отложенную очередь
            int count = Tracks.Count(t => t.User == track.User);
            if (count >= userLimit)
            {
                DeferredTracks.Add(track);

                int deferredTracksCount = DeferredTracks.Count(t => t.User == track.User);

                // Сохранение очередей в файл
                Save();

                return $"{track.User}, трек добавлен в отложенную очередь, {count + deferredTracksCount} треков всего";
            }

            TrackAddEvent?.Invoke(new TrackEventArgs {
                Track = track
            });

            Tracks.Add(track);

            // Сохранение очередей в файл
            Save();

            return $"{track.User}, трек добавлен.";
        }

        /// <summary>
        /// Удаление трека
        /// </summary>
        public void RemoveTrack(RocksmithTrack track)
        {
            if (!Tracks.Contains(track))
                return;

            Tracks.Remove(track);

            TrackRemoveEvent?.Invoke(new TrackEventArgs {
                Track = track
            });

            // После удаления из очереди проверяем треки, которые можно добавить из отложенных
            RocksmithTrack addTrack = DeferredTracks.FirstOrDefault(t => Tracks.Count(tt => tt.User == t.User) < userLimit);
            if (addTrack != null)
            {
                DeferredTracks.Remove(addTrack);
                Tracks.Add(addTrack);

                TrackAddEvent?.Invoke(new TrackEventArgs {
                    Track = addTrack
                });
            }

            // Сохранение очередей в файл
            Save();
        }

        public RocksmithQueue(string workDir, int limit)
        {
            saveLock = new object();
            rocksmithDir = workDir;
            userLimit = limit;

            // Треки
            Tracks = new List<RocksmithTrack>();

            // Треки для очереди сверх лимита
            DeferredTracks = new List<RocksmithTrack>();

            Load();
        }

        #endregion Основные функции
    }
}
