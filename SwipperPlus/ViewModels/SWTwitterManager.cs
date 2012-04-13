using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TweetSharp;
using Codeplex.OAuth;
using SwipperPlus.Models;
using SwipperPlus.Settings;
using SwipperPlus.Utils;

namespace SwipperPlus.ViewModels
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
      if (SWTwitterSettings.HasAccessToken())
      {
        twitter = new TwitterService(SWTwitterSettings.ConsumerKey, SWTwitterSettings.ConsumerSecret);
        AccessToken token = SWTwitterSettings.GetAccessToken();
        twitter.AuthenticateWith(token.Key, token.Secret);
      }
    }

    public override bool HasValidAccessToken()
    {
      return twitter != null;
    }

    /// <summary>
    /// Get new feeds from Twitter and parses the result
    /// </summary>
    public override void FetchFeeds()
    {
      this.twitter.ListTweetsOnHomeTimeline(10, (IEnumerable<TwitterStatus> statuses, TwitterResponse response) =>
        {
          if (response.StatusCode == HttpStatusCode.OK)
          {
            System.Diagnostics.Debug.WriteLine(response.StatusDescription);
            Feeds = new List<TwitterStatus>();
            foreach (TwitterStatus tweet in statuses)
              Feeds.Add(tweet);
            OnRaiseFeedsEvent(new SocialLinkEventArgs(null));
          }
          else
          {
            OnRaiseFeedsEvent(new SocialLinkEventArgs("Cannot receive Twitter feeds"));
          }
        }
      );
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
  }
}
