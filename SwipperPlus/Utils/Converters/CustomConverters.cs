using System;
using System.Windows;
using System.Web;
using System.Windows.Data;
using System.Globalization;
using SwipperPlus.Model.Facebook;

namespace SwipperPlus.Utils.Converters
{
  /// <summary>
  /// Converts an url into one that can be open by our in app browser
  /// </summary>
  public class UriForBrowserConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string uri = HttpUtility.UrlEncode(((Uri)value).ToString());
      return new Uri("/Views/SWLinkBrowser.xaml?type=link&value=" + uri);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
  }

  public class ImageToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is FacebookAttachment)
      {
        if (((FacebookAttachment)value).Type == FacebookAttachment.MediaType.Image)
          return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
  }

  public class LinkToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is FacebookAttachment)
      {
        if (((FacebookAttachment)value).Type == FacebookAttachment.MediaType.Link)
          return Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
  }
}
