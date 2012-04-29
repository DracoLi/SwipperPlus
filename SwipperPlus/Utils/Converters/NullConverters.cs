using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace SwipperPlus.Utils.Converters
{
  public class NullToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Visibility result = Visibility.Collapsed;
      if (value is string)
        result = value == null || (string)value == String.Empty ? Visibility.Visible : Visibility.Collapsed;
      else
        result = value == null ? Visibility.Visible : Visibility.Collapsed;
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class NotNullToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Visibility result = Visibility.Collapsed;
      if (value is string)
        result = value == null || (string)value == String.Empty ? Visibility.Collapsed : Visibility.Visible; 
      else
        result = value == null ? Visibility.Collapsed : Visibility.Visible; 
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
