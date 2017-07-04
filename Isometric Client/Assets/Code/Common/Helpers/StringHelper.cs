namespace Assets.Code.Common.Helpers
{
    public static class StringHelper
    {
        public static string FormatBy(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}