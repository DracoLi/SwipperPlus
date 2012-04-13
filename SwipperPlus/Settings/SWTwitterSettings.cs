using System;
using System.Collections.Generic;
using SwipperPlus.Utils;
using TweetSharp;
using Codeplex.OAuth;

namespace SwipperPlus.Settings
{
  public class SWTwitterSettings
  {
    // Twitter configs
    public const string ConsumerKey = "YGBtwcvHtB23zCiqZVA";
    public const string ConsumerSecret = "rNRkqWmjGYzXSZpTyijrQV6zGOqKqh3WMgx8fFWeWdU";

    public const string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
    public const string AuthorizeUri = "https://api.twitter.com/oauth/authorize";
    public const string AccessTokenUri = "https://api.twitter.com/oauth/access_token";
    public const string CallbackUri = "http://www.dracoli.com/twitter";
    public const string Authority = "http://api.twitter.com";

    // Constants
    const string AccessToken = "TwitterAccessToken";
    const string AccessTokenSecret = "AccessTokenSecret";
    const string RequestToken = "TwitterRequestToken";

    public static bool HasAccessToken()
    {
      return GetAccessToken() != null;
    }

    public static void RemoveAccessTokens()
    {
      Storage.SetKeyValue<OAuthAccessToken>(AccessToken, null);
    }

    public static void SetAccessToken(OAuthAccessToken token)
    {
      Storage.SetKeyValue<string>(AccessToken, token.Token);
      Storage.SetKeyValue<string>(AccessTokenSecret, token.TokenSecret);
    }

    public static AccessToken GetAccessToken()
    {
      string token = Storage.GetKeyValue<string>(AccessToken);
      string secret = Storage.GetKeyValue<string>(AccessTokenSecret);
      return new AccessToken(token, secret);
    }

    public static bool IsTwitterCallback(Uri url)
    {
      return url.ToString().ToLower().Contains(CallbackUri);
    }
  }
}
