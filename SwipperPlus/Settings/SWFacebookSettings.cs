using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;
using System.Collections.Generic;
using System.Text;

namespace SwipperPlus.Settings
{
  public static class SWFacebookSettings
  {
    // Facebook configs
    private const string AppID = "360095330708489";
    private const string AppSecret = "3b90796ee141966c6e010007ca4dfbbc";

    // Constants
    private const string AccessToken = "FacebookAccessToken";

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
      return !string.IsNullOrEmpty(GetAccessToken());
    }

    public static string GetAccessToken()
    { 
      return Storage.GetKeyValue<string>(AccessToken);
    }

    public static void SaveAccessToken(string token)
    {
      Storage.SetKeyValue<string>(AccessToken, token);
    }

    public static void RemoveAccessTokens()
    {
      Storage.SetKeyValue<string>(AccessToken, null);
    }
  }
}
