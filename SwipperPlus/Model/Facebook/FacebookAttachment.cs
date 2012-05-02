using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// This represents an attachment for a feed
  /// To simplify things, if there's more than one media, we take first one
  ///  That is for both links and images. Only link and images medias are supported right now
  /// </summary>
  public class FacebookAttachment
  {
    /// <summary>
    /// Different media types an attachment can be
    /// </summary>
    public static class MediaType
    {
      public static string Link = "Link";
      public static string Image = "Image";
      public static string NotSupported = "NotSupported";
    }

    /// <summary>
    /// The real url of the attachment
    /// </summary>
    public Uri Source { get; set; }

    public Uri Icon { get; set; }

    /// <summary>
    /// The type of the attachment
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Name of attachment, only used for links right now
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of attachment, used by links
    /// </summary>
    public string Description { get; set; }
  }
}
