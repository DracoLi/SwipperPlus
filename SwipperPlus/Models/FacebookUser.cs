using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Models
{
  /// <summary>
  /// Represents a person
  /// </summary>
  public class FacebookUser
  {
    public string ID;
    public string Name;
    public Uri Icon;

    public FacebookUser(string id)
    {
      ID = id;
    }

    public FacebookUser(string id, string name, Uri icon)
    {
      ID = id;
      Name = name;
      Icon = icon;
    }
  }
}
