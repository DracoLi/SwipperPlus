using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

namespace SwipperPlus.Models
{
  public static class People
  {
    public static Dictionary<string, FacebookUser> FBPeople = new Dictionary<string,FacebookUser>();
    public static Dictionary<string, FacebookUser> LinkedInPeople = new Dictionary<string, FacebookUser>();
  }
}
