using System;
using System.Collections.Generic;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// Different feed types supported by us
  /// </summary>
  public enum FeedType { NotSupported, Text, Attachment, Conversation, Action };

  /// <summary>
  /// This is the base class for a feed
  /// </summary>
  public class FacebookFeed
  {
    /// <summary>
    /// Can be Text, Attachment, Conversation, etc
    /// This will determine how the UI handles the feed
    /// </summary>
    public FeedType FeedType { get; set; }

    /// <summary>
    /// ID of the feed, directly from the source
    /// </summary>
    public string ID { get; set; }

    public DateTime Date { get; set; }

    /// <summary>
    /// Id of the person who posted it
    /// </summary>
    public UInt64 SourcePerson { get; set; }

    /// <summary>
    /// ID of the person who the feed is for. Used for conversation feeds
    /// </summary>
    public UInt64 TargetPerson { get; set; }

    /// <summary>
    /// This is the original message pared into rich text box xml format
    /// </summary>
    public string XmlMessage { get; set; }

    /// <summary>
    /// The original message for the feed
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// This corresponds to an action
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Handles likes & comments.
    /// Things that only Facebook have
    /// </summary>
    public FacebookItem SocialProperties { get; set; }

    /// <summary>
    /// Contains information about an attachment
    /// </summary>
    public FacebookAttachment Attachment { get; set; }
  }
}
