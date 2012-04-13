using System;
using System.Net;
using SwipperPlus.Utils;
using Codeplex.OAuth;

namespace SwipperPlus.Settings
{
  public class SWLinkedInSettings
  {
    // Twitter configs
    public const string ConsumerKey = "8qzh823wvn3k";
    public const string ConsumerSecret = "MaatV6x5kkNMr35S";

    public const string RequestTokenUri = "https://api.linkedin.com/uas/oauth/requestToken";
    public const string AuthorizeUri = "https://www.linkedin.com/uas/oauth/authenticate";
    public const string AccessTokenUri = "https://api.linkedin.com/uas/oauth/accessToken";
    public const string CallbackUri = "http://www.dracoli.com/linkedin";
    public const string Authority = "https://api.linkedin.com";

    // Constants
    const string AccessToken = "LinkedInAccessToken";
    const string AccessTokenSecret = "LinkedInAccessTokenSecret";

    public static bool HasAccessToken()
    {
      return GetAccessToken() != null;
    }

    public static void RemoveAccessTokens()
    {
      Storage.SetKeyValue<string>(AccessToken, null);
      Storage.SetKeyValue<string>(AccessTokenSecret, null);
    }

    public static void SetAccessToken(AccessToken token)
    {
      Storage.SetKeyValue<string>(AccessToken, token.Key);
      Storage.SetKeyValue<string>(AccessTokenSecret, token.Secret);
    }

    public static AccessToken GetAccessToken()
    {
      string token = Storage.GetKeyValue<string>(AccessToken);
      string secret = Storage.GetKeyValue<string>(AccessTokenSecret);
      return new AccessToken(token, secret);
    }

    public static bool IsLinkedInCallback(Uri url)
    {
      return url.ToString().ToLower().Contains(CallbackUri);
    }
  }
}
