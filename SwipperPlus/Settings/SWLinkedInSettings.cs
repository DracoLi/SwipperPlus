using System;
using Codeplex.OAuth;
using SwipperPlus.Utils;

namespace SwipperPlus.Settings
{
  public class SWLinkedInSettings
  {
    // Twitter configs
    public const string ConsumerKey           = "8qzh823wvn3k";
    public const string ConsumerSecret        = "MaatV6x5kkNMr35S";

    public const string RequestTokenUri       = "https://api.linkedin.com/uas/oauth/requestToken";
    public const string AuthorizeUri          = "https://www.linkedin.com/uas/oauth/authenticate";
    public const string AccessTokenUri        = "https://api.linkedin.com/uas/oauth/accessToken";
    public const string CallbackUri           = "http://www.dracoli.com/linkedin";
    public const string Authority             = "https://api.linkedin.com";

    // Constants
    private const string LinkedInAccessToken  = "LinkedInAccessToken";
    private const string LinkedInAccessSecret = "LinkedInAccessSecret";
    private const string LinkedInEnabled      = "LinkedInEnabled";

    public static bool IsConnected()
    {
      return StorageUtils.HasKeyValue(LinkedInAccessToken)
        && StorageUtils.HasKeyValue(LinkedInAccessSecret);
    }

    public static bool IsEnabled()
    {
      return StorageUtils.HasKeyValue(LinkedInEnabled);
    }

    public static void SetEnabled(bool enabled)
    {
      if (enabled)  StorageUtils.SetKeyValue<bool>(LinkedInEnabled, true);
      else          StorageUtils.RemoveKeyValue(LinkedInEnabled);
    }

    public static AccessToken GetAccessToken()
    {
      if (!IsConnected()) return null;

      string key = StorageUtils.GetKeyValue<string>(LinkedInAccessToken);
      string secret = StorageUtils.GetKeyValue<string>(LinkedInAccessSecret);
      return new AccessToken(key, secret);
    }

    public static void SetAccessToken(AccessToken token)
    {
      StorageUtils.SetKeyValue<string>(LinkedInAccessToken, token.Key);
      StorageUtils.SetKeyValue<string>(LinkedInAccessSecret, token.Secret);
    }

    public static void RemoveAccessToken()
    {
      StorageUtils.RemoveKeyValue(LinkedInAccessToken);
      StorageUtils.RemoveKeyValue(LinkedInAccessSecret);
    }

    /// <summary>
    /// Determines if an uri is a linkedin authorization callback
    /// </summary>
    public static bool IsLinkedInCallback(Uri url)
    {
      return url.ToString().ToLower().Contains(CallbackUri);
    }
  }
}
