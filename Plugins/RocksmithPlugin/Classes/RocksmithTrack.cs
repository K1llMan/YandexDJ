using System;

namespace RocksmithPlugin.Classes
{
    /// <summary>
    /// Описание трека заказа
    /// </summary>
    public class RocksmithTrack: IEquatable<RocksmithTrack>
    {
        public string Key { get; set; }

        public string Artist { get; set; }

        public string Name { get; set; }

        public string User { get; set; }

        public string ArrangementType { get; set; }

        #region IEquatable

        public bool Equals(RocksmithTrack other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RocksmithTrack) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key);
        }

        #endregion IEquatable
    }
}
