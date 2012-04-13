using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;
using System.Collections.Generic;
using System.Text;

namespace SwipperPlus.Utils
{
  public static class SWFacebookUtils
  {
    // Facebook configs
    public const string AppID = "360095330708489";
    public const string AppSecret = "3b90796ee141966c6e010007ca4dfbbc";

    // Constants
    const string FacebookAccessToken = "FacebookAccessToken";

    public static Dictionary<string, object> GetLoginParameters()
    {
      var loginParameters = new Dictionary<string, object>
      {
        {"client_id", AppID},
        {"response_type", "token"},
        {"display", "touch"},
        {"scope", "read_stream,user_groups,manage_notifications"},
        {"redirect_uri", "http://www.facebook.com/connect/login_success.html"}
      };
      return loginParameters;
    }

    public static bool HasAccessToken()
    {
      return !string.IsNullOrEmpty(SWFacebookUtils.GetAccessToken());
    }

    public static string GetAccessToken()
    { 
      return Storage.GetKeyValue<string>(FacebookAccessToken);
    }

    public static void SaveAccessToken(string token)
    {
      Storage.SetKeyValue<string>(FacebookAccessToken, token);
    }

    public static void RemoveAccessTokens()
    {
      Storage.SetKeyValue<string>(FacebookAccessToken, null);
    }
  }
}
