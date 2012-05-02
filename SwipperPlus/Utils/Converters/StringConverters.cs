using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace SwipperPlus.Utils.Converters
{
  public class StringToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return String.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
