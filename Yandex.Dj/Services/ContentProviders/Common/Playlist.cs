namespace Yandex.Dj.Services.ContentProviders.Common
{
    public class Playlist
    {
        #region Свойства

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string ID;
        /// <summary>
        /// Тип провайдера
        /// </summary>
        public ProviderType Type;
        /// <summary>
        /// Обложка
        /// </summary>
        public string Cover;
        /// <summary>
        /// Название
        /// </summary>
        public string Title;
        /// <summary>
        /// Список треков в плейлисте
        /// </summary>
        public Track[] Tracks;

        #endregion Свойства
    }
}
