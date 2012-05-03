using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TweetSharp;
using Codeplex.OAuth;
using SwipperPlus.Model.Twitter;
using SwipperPlus.Utils;
using SwipperPlus.Utils.Parsers;
using SwipperPlus.Model.Facebook;
using SwipperPlus.Settings;
using SwipperPlus.Views;

namespace SwipperPlus.ViewModel
{
  /**
   * Connects app with Twitter if user authenticated
   */
  public class SWTwitterManager : SWSocialLinkManager
  {
    public ObservableCollection<SWTwitterFeedList> Feeds { set; get; }

    public override string Title
    {
      get { return "Twitter"; }
    }

    private TwitterService twitter;

    public SWTwitterManager()
    {
      if (!SWTwitterSettings.IsConnected())
        throw new Exception("Social link must be connected!");

      // Set up feeds
      Feeds = new ObservableCollection<SWTwitterFeedList>();
      SWTwitterFeedList list = new SWTwitterFeedList();
      Feeds.Add(list);
      LoadSavedFeeds();

      // Set up others
      twitter = new TwitterService(SWTwitterSettings.ConsumerKey, SWTwitterSettings.ConsumerSecret);
      AccessToken token = SWTwitterSettings.GetAccessToken();
      twitter.AuthenticateWith(token.Key, token.Secret);
      IsUpdating = false;
    }

    public override int FeedCount
    { 
      get { return Feeds[0].Count; }
    }

    /// <summary>
    /// Get new feeds from Twitter and parses the result
    /// </summary>
    public override void FetchFeeds()
    {
      if (IsUpdating) return;

      // Make sure we don't fetch
      IsUpdating = true;
      CurrentAction = FeedAction.New;
      twitter.ListTweetsOnHomeTimeline(GeneralSettings.InitialFeedsToGet, tw_HandleCallback);
    }

    public override void GetMoreFeeds()
    {
      if (IsUpdating) return;

      if (Feeds[0].Count == 0)
        FetchFeeds();
      else
      {
        IsUpdating = true;
        CurrentAction = FeedAction.More;
        twitter.ListTweetsOnHomeTimelineBefore(Feeds[0][Feeds[0].Count-1].ID,
          GeneralSettings.InitialFeedsToGet, tw_HandleCallback);
      }
    }

    public override void SaveFeeds()
    {
      StorageUtils.SetKeyValue<SWTwitterFeedList>("TwitterFeeds", Feeds[0]);
    }

    public override void LoadSavedFeeds()
    {
      if (StorageUtils.HasKeyValue("TwitterFeeds"))
        Feeds[0] = StorageUtils.GetKeyValue<SWTwitterFeedList>("TwitterFeeds");
    }

    private void tw_HandleCallback(IEnumerable<TwitterStatus> statuses, TwitterResponse response)
    {
      if (response.StatusCode == HttpStatusCode.OK)
      {
        parseTwitterStatuses(statuses);
      }
      else
      {
        IsUpdating = false;
        OnRaiseFeedsEvent(new SocialLinkEventArgs("Cannot receive Twitter feeds"));
      }
    }

    private void parseTwitterStatuses(IEnumerable<TwitterStatus> statuses)
    {
      Deployment.Current.Dispatcher.BeginInvoke(() =>
      {
        // Determine feed status
        if (CurrentAction == FeedAction.New)
          Feeds[0].Clear();

        // Save retrieved twitter feeds
        bool isFirst = true;
        foreach (TwitterStatus tweet in statuses)
        {
          // Skip first of getting more
          if (CurrentAction == FeedAction.More && isFirst)
          {
            isFirst = false;
            continue;
          }
          Feeds[0].Add(TwitterParser.ParseTweet(tweet));
        }

        // Save feeds if this is a refresh
        if (CurrentAction == FeedAction.New)
          SaveFeeds();

        IsUpdating = false;
        LastUpdated = DateTime.Now;
        OnRaiseFeedsEvent(new SocialLinkEventArgs(CurrentAction));
      });
    }
  }
}
