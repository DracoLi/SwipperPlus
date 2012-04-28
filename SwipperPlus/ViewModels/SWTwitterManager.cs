using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TweetSharp;
using Codeplex.OAuth;
using SwipperPlus.Model.Twitter;
using SwipperPlus.Utils;
using SwipperPlus.Utils.Parsers;
using SwipperPlus.Model.Facebook;
using SwipperPlus.Settings;

namespace SwipperPlus.ViewModel
{
  /**
   * Connects app with Twitter if user authenticated
   */
  public class SWTwitterManager : SWSocialLinkManager
  {
    public ObservableCollection<SWTwitterFeed> Feeds { private set; get; }

    public override string Title
    {
      get { return "Twitter"; }
    }

    private TwitterService twitter;

    public SWTwitterManager()
    {
      twitter = new TwitterService(SWTwitterSettings.ConsumerKey, SWTwitterSettings.ConsumerSecret);
      AccessToken token = SWTwitterSettings.GetAccessToken();
      twitter.AuthenticateWith(token.Key, token.Secret);
    }

    /// <summary>
    /// Get new feeds from Twitter and parses the result
    /// </summary>
    public override void FetchFeeds()
    {
      Feeds = new ObservableCollection<SWTwitterFeed>();
      twitter.ListTweetsOnHomeTimeline(GeneralSettings.FeedsToGet, tw_HandleCallback);
    }

    public override void UpdateFeeds()
    {
      twitter.ListTweetsOnHomeTimelineSince(Feeds[0].ID, tw_HandleCallback);
    }

    public override void SaveFeeds()
    {
      
    }

    public override bool HaveSavedFeeds()
    {
      bool result = false;

      return result;
    }

    public override void LoadSavedFeeds()
    {
      
    }

    private void tw_HandleCallback(IEnumerable<TwitterStatus> statuses, TwitterResponse response)
    {
      if (response.StatusCode == HttpStatusCode.OK)
      {
        // Determine feed status
        FeedStatus status = FeedStatus.New;
        ObservableCollection<SWTwitterFeed> oldFeeds = null;
        if (Feeds.Count != 0)
        {
          status = FeedStatus.Updated;
          oldFeeds = Feeds;
          Feeds = new ObservableCollection<SWTwitterFeed>();
        }

        // Save retrieved twitter feeds
        foreach (TwitterStatus tweet in statuses)
          Feeds.Add(TwitterParser.ParseTweet(tweet));

        // Add in the old feeds if its an update
        if (oldFeeds != null)
          foreach (SWTwitterFeed st in oldFeeds)
            Feeds.Add(st);

        OnRaiseFeedsEvent(new SocialLinkEventArgs(status));
      }
      else
      {
        OnRaiseFeedsEvent(new SocialLinkEventArgs("Cannot receive Twitter feeds"));
      }
    }
  }
}
