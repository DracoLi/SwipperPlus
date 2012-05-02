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
      twitter.ListTweetsOnHomeTimeline(GeneralSettings.InitialFeedsToGet, tw_HandleCallback);
    }

    public override void GetMoreFeeds()
    {
      throw new NotImplementedException();
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
        if (CurrentAction == FeedAction.New)
        {
          Feeds = new ObservableCollection<SWTwitterFeed>();
        }

        // Save retrieved twitter feeds
        foreach (TwitterStatus tweet in statuses)
          Feeds.Add(TwitterParser.ParseTweet(tweet));

        OnRaiseFeedsEvent(new SocialLinkEventArgs(CurrentAction));
      }
      else
      {
        OnRaiseFeedsEvent(new SocialLinkEventArgs("Cannot receive Twitter feeds"));
      }
    }
  }
}
