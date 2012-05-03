using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SwipperPlus.Settings
{
  public static class GeneralSettings
  {
    public const int InitialFeedsToGet = 30;
    public const int AdditionalFeedsToGet = 10;

    // The delay to fetch new feeds right after fetching some feeds
    public const int FetchDelay = 1000;
    public const int RefreshDelay = 1000 * 60 * 60; // 1hr
  }
}
