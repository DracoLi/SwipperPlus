using System;
using System.Collections.Generic;
using Codeplex.OAuth;
using SwipperPlus.Utils;

namespace SwipperPlus.Settings
{
  public static class SWFacebookSettings
  {
    // Facebook configs
    public const string ConsumerKey           = "360095330708489";
    public const string ConsumerSecret        = "3b90796ee141966c6e010007ca4dfbbc";

    // Constants
    private const string FacebookAccessToken  = "FacebookAccessToken";

    public static Dictionary<string, object> GetLoginParameters()
    {
      var loginParameters = new Dictionary<string, object>
      {
        {"client_id", ConsumerKey},
        {"response_type", "token"},
        {"display", "touch"},
        {"scope", "read_stream,user_groups,manage_notifications"},
        {"redirect_uri", "http://www.facebook.com/connect/login_success.html"}
      };
      return loginParameters;
    }

    public static bool IsConnected()
    {
      return StorageUtils.HasKeyValue(FacebookAccessToken);
    }

    public static string GetAccessToken()
    {
      if (!IsConnected()) return null;
      return StorageUtils.GetKeyValue<string>(FacebookAccessToken);
    }

    public static void SetAccessToken(string token)
    {
      StorageUtils.SetKeyValue<string>(FacebookAccessToken, token);
    }

    public static void RemoveAccessToken()
    {
      StorageUtils.RemoveKeyValue(FacebookAccessToken);
    }
  }
}
