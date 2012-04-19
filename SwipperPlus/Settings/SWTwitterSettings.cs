using System;
using Codeplex.OAuth;
using SwipperPlus.Utils;

namespace SwipperPlus.Settings
{
  public class SWTwitterSettings
  {
    // Twitter configs
    public const string ConsumerKey         = "YGBtwcvHtB23zCiqZVA";
    public const string ConsumerSecret      = "rNRkqWmjGYzXSZpTyijrQV6zGOqKqh3WMgx8fFWeWdU";

    public const string RequestTokenUri     = "https://api.twitter.com/oauth/request_token";
    public const string AuthorizeUri        = "https://api.twitter.com/oauth/authorize";
    public const string AccessTokenUri      = "https://api.twitter.com/oauth/access_token";
    public const string CallbackUri         = "http://www.dracoli.com/twitter";
    public const string Authority           = "http://api.twitter.com";

    // Constants
    private const string TwitterAccessToken   = "TwitterAccessToken";
    private const string TwitterAccessSecret  = "TwitterAccessSecret";
    private static string TwitterEnabled      = "TwitterEnabled";

    public static bool IsConnected()
    {
      return StorageUtils.HasKeyValue(TwitterAccessToken) 
        && StorageUtils.HasKeyValue(TwitterAccessSecret);
    }

    public static bool IsEnabled()
    {
      return StorageUtils.HasKeyValue(TwitterEnabled);
    }

    public static void SetEnabled(bool enabled)
    {
      if (enabled) StorageUtils.SetKeyValue<bool>(TwitterEnabled, true);
      else StorageUtils.RemoveKeyValue(TwitterEnabled);
    }

    public static AccessToken GetAccessToken()
    {
      if (!IsConnected()) return null;

      string key = StorageUtils.GetKeyValue<string>(TwitterAccessToken);
      string secret = StorageUtils.GetKeyValue<string>(TwitterAccessSecret);
      return new AccessToken(key, secret);
    }

    public static void SetAccessToken(AccessToken token)
    {
      StorageUtils.SetKeyValue<string>(TwitterAccessToken, token.Key);
      StorageUtils.SetKeyValue<string>(TwitterAccessSecret, token.Secret);
    }

    public static void RemoveAccessToken()
    {
      StorageUtils.RemoveKeyValue(TwitterAccessToken);
      StorageUtils.RemoveKeyValue(TwitterAccessSecret);
    }

    /// <summary>
    /// Determine if an url is a twitter callback
    /// </summary>
    public static bool IsTwitterCallback(Uri url)
    {
      return url.ToString().ToLower().Contains(CallbackUri);
    }
  }
}
