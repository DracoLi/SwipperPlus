using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// This represents an attachment for a feed
  /// </summary>
  public class FacebookAttachment
  {
    /// <summary>
    /// Different media types an attachment can be
    /// </summary>
    public enum MediaType { Video, Link, Image };

    /// <summary>
    /// The real url of the attachment
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// The type of the attachment
    /// </summary>
    public MediaType Type { get; set; }

    /// <summary>
    /// Name of attachment
    /// </summary>
    public string Name { get; set; }
    public Uri Icon { get; set; }
  }
}
