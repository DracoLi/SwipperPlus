using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using SwipperPlus.Utils;

namespace SwipperPlus.Utils.Converters
{
  /// <summary>
  /// Converts bool to Visibility
  /// True = Visible, False = Collapse
  /// </summary>
  public class BoolToVisibleConverter : IValueConverter
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
  
  /// <summary>
  /// Converts a bool to Visibility.
  /// True = Collapse, False = Visible
  /// </summary>
  public class BoolToCollapseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return ((bool)value) ? Visibility.Collapsed : Visibility.Visible;
      }
      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility)
      {
        return ((Visibility)value) == Visibility.Collapsed ? true : false;
      }
      return false;
    }
  }

  /// <summary>
  /// Converts a bool to string for is enabled state
  /// </summary>
  public class BoolToIsEnabledConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return ((bool)value) ? "Enabled" : "Disabled";
      }
      return "Disabled";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string)
      {
        return ((string)value).Equals("Enabled") ? true : false;
      }
      return false;
    }
  }

  public class BoolToConnectTextConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return ((bool)value) ? "Disconnect" : "Connect";
      }
      return "Connect";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string)
      {
        return ((string)value).Equals("Disconnect") ? true : false;
      }
      return false;
    }
  }

  public class BoolToConnectBrushConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return GeneralUtils.GetColorFromHex("#FF30BA00");
      }
      return GeneralUtils.GetColorFromHex("#FF30BA00");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is string)
      {
        return ((string)value).Equals("FF30BA00") ? false : true;
      }
      return false;
    }
  }
}
