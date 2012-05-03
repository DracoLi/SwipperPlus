using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TweetSharp;

namespace SwipperPlus.Model.Twitter
{
  [DataContract]
  public class SWTwitterFeed
  {
    /// <summary>
    /// ID of the feed, directly from the source
    /// </summary>
    [DataMember]
    public long ID { get; set; }

    [DataMember]
    public DateTime Date { get; set; }

    /// <summary>
    /// The user who tweeted it or retweeted it.
    /// </summary>
    [DataMember]
    public SWTwitterUser TweetedUser { get; set; }

    /// <summary>
    /// If this is a retweet, OriginalUser is the person who originally tweeted it.
    /// If not a retweet this is same as TweetedUser
    /// </summary>
    [DataMember]
    public SWTwitterUser OriginalUser { get; set; }

    [DataMember]
    public bool IsRetweet { get; set; }

    /// <summary>
    /// This is the original message pared into rich text box xml format
    /// </summary>
    [DataMember]
    public string XmlMessage { get; set; }

    /// <summary>
    /// The original message for the feed
    /// </summary>
    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public IList<SWTag> MessageTags { get; set; }

    /// <summary>
    /// Uri of any links
    /// </summary>
    [DataMember]
    public Uri PhotoUrl { get; set; }
  }
}
