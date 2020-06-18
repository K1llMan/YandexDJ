namespace Yandex.Dj.Services.ContentProviders.Common
{
    public class Track
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
        /// Исполнитель
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        #endregion Свойства
    }
}
