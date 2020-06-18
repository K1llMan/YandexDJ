namespace Yandex.Dj.Services.ContentProviders.Common
{
    public class Playlist
    {
        #region Свойства

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Тип провайдера
        /// </summary>
        public ProviderType Type { get; set; }
        /// <summary>
        /// Обложка
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Список треков в плейлисте
        /// </summary>
        public Track[] Tracks { get; set; }

        #endregion Свойства
    }
}
