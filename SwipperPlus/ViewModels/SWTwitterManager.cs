using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TweetSharp;
using Codeplex.OAuth;
using SwipperPlus.Model;
using SwipperPlus.Settings;
using SwipperPlus.Utils;

namespace SwipperPlus.Model
{
  /**
   * Connects app with Twitter if user authenticated
   */
  public class SWTwitterManager : SWSocialLinkManager
  {
    public List<TwitterStatus> Feeds { private set; get; }

    private TwitterService twitter;

    public SWTwitterManager()
    {
      twitter = new TwitterService(SWTwitterSettings.ConsumerKey, SWTwitterSettings.ConsumerSecret);
      AccessToken token = SWTwitterSettings.GetAccessToken();
      twitter.AuthenticateWith(token.Key, token.Secret);
    }

    public static bool IsConnected()
    {
      return SWTwitterSettings.IsConnected();
    }

    /// <summary>
    /// Get new feeds from Twitter and parses the result
    /// </summary>
    public override void FetchFeeds()
    {
      Feeds = new List<TwitterStatus>();
      this.twitter.ListTweetsOnHomeTimeline(GeneralSettings.FeedsToGet, tw_HandleCallback);
    }

    public override void UpdateFeeds()
    {
      this.twitter.ListTweetsOnHomeTimelineSince(Feeds[0].Id, tw_HandleCallback);
    }

    public override void SaveFeeds()
    {
      throw new NotImplementedException();
    }

    public override bool HaveSavedFeeds()
    {
      throw new NotImplementedException();
    }

    public override void LoadSavedFeeds()
    {
      throw new NotImplementedException();
    }

    private void tw_HandleCallback(IEnumerable<TwitterStatus> statuses, TwitterResponse response)
    {
      if (response.StatusCode == HttpStatusCode.OK)
      {
        // Determine feed status
        FeedStatus status = Feeds.Count == 0 ? FeedStatus.New : FeedStatus.Updated;

        // Save retrieved twitter feeds
        List<TwitterStatus> tempFeeds = new List<TwitterStatus>();
        foreach (TwitterStatus tweet in statuses)
          tempFeeds.Add(tweet);

        // Add new feeds to top of list
        Feeds.InsertRange(0, tempFeeds);

        OnRaiseFeedsEvent(new SocialLinkEventArgs(status));
      }
      else
      {
        OnRaiseFeedsEvent(new SocialLinkEventArgs("Cannot receive Twitter feeds"));
      }
    }
  }
}
