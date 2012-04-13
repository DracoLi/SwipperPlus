using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Models
{
  public class Person
  {
    public UInt64 ID;
    public string Name;
    public Uri Icon;
    public SocialLinkType Type;

    public Person(UInt64 id, SocialLinkType type)
    {
      ID = id;
      Type = type;
    }

    public Person(UInt64 id, string name, Uri icon, SocialLinkType type)
    {
      ID = id;
      Name = name;
      Icon = icon;
      Type = type;
    }
  }
}
