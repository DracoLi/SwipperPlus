using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// Represents a Facebook person
  /// </summary>
  public class FacebookUser
  {
    public UInt64 ID { set; get; }
    public string DisplayName { set; get; }
    public Uri Icon { set; get; }

    public FacebookUser(UInt64 id)
    {
      ID = id;
    }

    public FacebookUser(UInt64 id, string name, Uri icon)
    {
      ID = id;
      DisplayName = name;
      Icon = icon;
    }
  }
}
