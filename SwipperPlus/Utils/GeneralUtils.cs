using System;
using System.Net;
using System.Windows;

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
  }
}
