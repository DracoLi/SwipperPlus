using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Models
{
  /// <summary>
  /// This represents an attachment for a feed
  /// </summary>
  public class Attachment
  {
    public enum MediaType { Video, Link, Image };

    // The real url of the attachment
    public Uri Href { get; set; }

    // The type of the attachment
    public MediaType Type { get; set; }
    public string Name { get; set; }
    public Uri Icon { get; set; }
  }
}
