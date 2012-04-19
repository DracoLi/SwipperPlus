using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// Represents a person
  /// </summary>
  public class FacebookUser
  {
    public UInt64 ID;
    public string Name;
    public Uri Icon;

    public FacebookUser(UInt64 id)
    {
      ID = id;
    }

    public FacebookUser(UInt64 id, string name, Uri icon)
    {
      ID = id;
      Name = name;
      Icon = icon;
    }
  }
}
