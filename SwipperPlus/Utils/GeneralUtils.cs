using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace SwipperPlus.Utils
{
  public static class GeneralUtils
  {
    internal static long DateTimeToUnixTimestamp(DateTime _DateTime)
    {
      TimeSpan _UnixTimeSpan = (_DateTime - new DateTime(1970, 1, 1, 0, 0, 0));
      return (long)_UnixTimeSpan.TotalSeconds;
    }

    internal static DateTime UnixTimestampToDateTime(long _UnixTimeStamp)
    {
      return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(_UnixTimeStamp);
    }

    /// <summary>
    /// Get only the query parameters of the url
    /// </summary>
    /// <returns>Dictionary of key-value queries</returns>
    internal static Dictionary<string, string> GetQueryParameters(Uri uri)
    {
      string url = uri.ToString();

      // Get only the query
      int iqs = url.IndexOf('?');
      if (iqs == -1) return null;
      string queryString = url.Substring(iqs + 1);

      // Get he parameters
      Dictionary<string, string> nameValueCollection = new Dictionary<string, string>();
      string[] items = queryString.Split('&');
      foreach (string item in items)
      {
        if (item.Contains("="))
        {
          string[] nameValue = item.Split('=');
          nameValueCollection.Add(HttpUtility.UrlDecode(nameValue[0]), HttpUtility.UrlDecode(nameValue[1]));
        }
      }
      return nameValueCollection;
    }

    internal static bool HasInternetConnection()
    {
      return NetworkInterface.GetIsNetworkAvailable();
    }

    /// <summary>
    /// Return a Color object from a hex code
    /// </summary>
    /// <param name="hexaColor"></param>
    /// <returns></returns>
    internal static SolidColorBrush GetColorFromHex(string hexaColor)
    {
      return new SolidColorBrush(
          Color.FromArgb(
              Convert.ToByte(hexaColor.Substring(1, 2), 16),
              Convert.ToByte(hexaColor.Substring(3, 2), 16),
              Convert.ToByte(hexaColor.Substring(5, 2), 16),
              Convert.ToByte(hexaColor.Substring(7, 2), 16)
          )
      );
    }

    /// <summary>
    /// Trim words into a specified count. Represented by ellipsis.
    /// </summary>
    internal static string TrimWords(string words, int length)
    {
      length = Math.Min(words.Length, length);
      return words.Substring(0, length) + "...";
    }

    /// <summary>
    /// Make url for a link that opens a website
    /// </summary>
    internal static Uri UriForWebsiteNavigation(string url)
    {
      return new Uri("/Views/UriBrowser.xaml?type=link&value=" + url, UriKind.Relative);
    }

    /// <summary>
    /// Return the uri needed to view the image
    /// </summary>
    internal static Uri UriForImageViewing(string large, string small = null)
    {
      string result = "/Views/ImageViewer.xaml?large=" + large;
      if (small != null)
        result += "&small=" + small;
      return new Uri(result, UriKind.Relative);
    }
  }
}
