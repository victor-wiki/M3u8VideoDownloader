using System.Web;

namespace M3u8VideoHandler
{
    public class UrlHelper
    {
        public static bool IsValidUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && (url.StartsWith("www") || url.StartsWith("http"));
        }       

        public static bool IsM3u8(string url)
        {
            return !string.IsNullOrEmpty(url) && url.ToLower().Contains("m3u8");
        }
    }
}
