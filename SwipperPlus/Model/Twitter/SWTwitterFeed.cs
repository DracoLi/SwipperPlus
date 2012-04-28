using System;
using System.Collections.Generic;
using TweetSharp;

namespace SwipperPlus.Model.Twitter
{
  public class SWTwitterFeed
  {
    /// <summary>
    /// ID of the feed, directly from the source
    /// </summary>
    public long ID { get; set; }

    public DateTime Date { get; set; }

    /// <summary>
    /// The user who tweeted it or retweeted it.
    /// </summary>
    public SWTwitterUser TweetedUser { get; set; }

    /// <summary>
    /// If this is a retweet, OriginalUser is the person who originally tweeted it.
    /// If not a retweet this is same as TweetedUser
    /// </summary>
    public SWTwitterUser OriginalUser { get; set; }

    public bool IsRetweet { get; set; }

    /// <summary>
    /// This is the original message pared into rich text box xml format
    /// </summary>
    public string XmlMessage { get; set; }

    /// <summary>
    /// The original message for the feed
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Uri of any links
    /// </summary>
    public IList<TwitterUrl> Urls { get; set; }
  }
}
