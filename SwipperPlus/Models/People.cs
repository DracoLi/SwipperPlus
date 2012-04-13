using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

namespace SwipperPlus.Models
{
  public static class People
  {
    public static Dictionary<UInt64, Person> FBPeople = new Dictionary<ulong,Person>();
    public static Dictionary<UInt64, Person> TwitterPeople = new Dictionary<ulong, Person>();
    public static Dictionary<UInt64, Person> LinkedInPeople = new Dictionary<ulong, Person>();
  }
}
