namespace Assets.Code.Common
{
    public class Settings
    {
        private static Settings _current;
        public static Settings Current
        {
            get { return _current != null ? _current : (_current = new Settings()); }
            set { _current = value; }
        }

        public bool ShowTips = true;

        public float GameSpeed = 1;
    }
}