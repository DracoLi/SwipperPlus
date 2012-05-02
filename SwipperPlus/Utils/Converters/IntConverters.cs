using System;
using System.Windows;
using System.Web;
using System.Windows.Data;
using System.Globalization;

namespace SwipperPlus.Utils.Converters
{
  public class IntToVisiblityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is int)
      {
        return ((int)value) == 0 ? Visibility.Collapsed : Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotImplementedException();
    }
  }
}
