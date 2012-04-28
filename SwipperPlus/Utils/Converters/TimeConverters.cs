using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using SwipperPlus.Utils.Parsers;

namespace SwipperPlus.Utils.Converters
{
  /// <summary>
  /// Converts DateTime to Twitter time
  /// </summary>
  public class DateTimeToTwitterTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is DateTime)
      {
        return TwitterParser.ParseTwitterDate((DateTime)value);
      }
      return "unknown";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Converts DateTime to Facebook time
  /// </summary>
  public class DateTimeToFacebookTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
      }
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility)
      {
        return ((Visibility)value) == Visibility.Visible ? true : false;
      }
      return false;
    }
  }
}
