using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;

namespace SwipperPlus.Models
{
  public enum FeedType { NotSupported, Text, Attachment, Conversation, Action };

  /// <summary>
  /// This is the base class for a feed
  /// </summary>
  public class SWFacebookFeed
  {
    // Can be Text, Attachment, Conversation, etc
    // This will determine how the UI handles the feed
    public FeedType FeedType { get; set; }

    // ID of the feed, directly from the source
    public string ID { get; set; }

    public DateTime Date { get; set; }

    // Id of the person who posted it
    public UInt64 SourcePerson { get; set; }

    // ID of the person who the feed is for. Used for conversation feeds
    public UInt64 TargetPerson { get; set; }

    public string Message { get; set; }

    // This corresponds to an action
    public string Description { get; set; }

    // Handles likes & comments.
    // Things that only Facebook have
    public FacebookItem FacebookProperties { get; set; }

    // Contains information about an attachment
    public Attachment AttachmentProperties { get; set; }
  }
}
