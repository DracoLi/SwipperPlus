using System;
using Newtonsoft.Json.Linq;
using SwipperPlus.Model.LinkedIn;

namespace SwipperPlus.Utils.Parsers
{
  public static class LinkedInParser
  {
    public static LinkedInFeed ParseFeed(JToken token)
    {
      LinkedInFeed result = new LinkedInFeed();


      return result;
    }

    private static LinkedInUser parseUser(JToken token)
    {
      LinkedInUser user = new LinkedInUser();
      return user;
    }

    /// <summary>
    /// A helper that tells us whether a JToken has any value
    /// </summary>
    private static bool tokenHasValue(JToken token)
    {
      if (token == null) return false;
      return !String.IsNullOrEmpty(token.ToString());
    }
  }
}
