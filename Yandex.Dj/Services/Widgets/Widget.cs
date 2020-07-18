namespace Yandex.Dj.Services.Widgets
{
    /// <summary>
    /// Виджет
    /// </summary>
    public class Widget
    {
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Visible { get; set; }
        public int Order { get; set; }
    }
}
