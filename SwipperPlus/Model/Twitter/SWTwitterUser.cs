using System;
using System.Windows;

namespace SwipperPlus.Model.Twitter
{
  /// <summary>
  /// Represents a Twitter user
  /// </summary>
  public class SWTwitterUser
  {
    public int ID { get; set; }
    public string DisplayName { get; set; }
    public Uri Icon { get; set; }
  }
}
