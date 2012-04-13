using System;
using System.Net;

namespace SwipperPlus.Models
{
  public class SWLinkedInFeed
  {
    public FeedType FeedType { get; set; }

    // ID of the feed, directly from the source
    public string ID { get; set; }

    public DateTime Date { get; set; }
  }
}
