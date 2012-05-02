using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facebook;
using Newtonsoft.Json.Linq;
using SwipperPlus.Utils;
using SwipperPlus.Utils.Parsers;
using SwipperPlus.Model.Facebook;
using SwipperPlus.Settings;

namespace SwipperPlus.ViewModel
{
  /// <summary>
  /// ViewModel for the Facebook feed
  /// </summary>
  public class SWFacebookManager : SWSocialLinkManager
  {
    /// <summary>
    /// Every facebook feed for the user
    /// </summary>
    public ObservableCollection<FacebookFeedList> Feeds { set; get; }

    /// <summary>
    /// This specifies the amount of feeds to get when user want more
    /// </summary>
    private int amountToGrab = 10;

    /// <summary>
    /// Title of this social link, used by pivot item header
    /// </summary>
    public override string Title
    {
      get { return "Facebook"; }
    }

    /// <summary>
    /// All facebook users are stored here to reduce duplicates
    /// </summary>
    public Dictionary<ulong, FacebookUser> People { private set; get; }

    private FacebookClient fb;

    public SWFacebookManager()
    {
      if (!SWFacebookSettings.IsConnected()) throw new Exception("Social link must be connected!");

      People = new Dictionary<ulong, FacebookUser>();
      fb = new FacebookClient(SWFacebookSettings.GetAccessToken());
      fb.GetCompleted += new EventHandler<FacebookApiEventArgs>(fb_GetCompleted);
    }

    /// <summary>
    /// Get new feeds from facebook and parses the result async
    /// </summary>
    public override void FetchFeeds()
    {
      // Do this so that we know we are fetching new feeds
      CurrentAction = FeedAction.New;
      string q1, q2, q3;
      getQueries(out q1, out q2, out q3);
      string[] queries = { q1, q2, q3 };
      fb.QueryAsync(queries);
    }

    /// <summary>
    /// Get more feeds
    /// </summary>
    /// <param name="amount"></param>
    public override void GetMoreFeeds()
    {
      // Update new feeds if we have some feeds already
      if (Feeds != null && Feeds.Count > 0 && Feeds[0].Count > 0)
      {
        long lastTime = GeneralUtils.DateTimeToUnixTimestamp(Feeds[0][Feeds.Count-1].Date);
        CurrentAction = FeedAction.More;
        string q1, q2, q3;
        getQueries(out q1, out q2, out q3, lastTime);
        string[] queries = { q1, q2, q3 };
        fb.QueryAsync(queries);
      }
      else
      {
        // If we have no feeds right now, just fetch instead
        FetchFeeds();
      }
    }

    public override void SaveFeeds()
    {
      throw new NotImplementedException();
    }

    public override void LoadSavedFeeds()
    {
      throw new NotImplementedException();
    }

    public override bool HaveSavedFeeds()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Called whenever we get feeds from facebook
    /// </summary>
    private void fb_GetCompleted(object sender, FacebookApiEventArgs e)
    {
      // Handle feeds retreival error
      if (e.Error != null)
      {
        System.Diagnostics.Debug.WriteLine(e.Error.Message);
        OnRaiseFeedsEvent(new SocialLinkEventArgs(e.Error.Message));
        return;
      }

      // Parse json results to facebook feeds
      System.Diagnostics.Debug.WriteLine(e.GetResultData().ToString());
      JArray rawFeeds = JArray.Parse(e.GetResultData().ToString());

      // Update our people list (add person if not in there already)
      for (int i = 1; i < 3; i++)
      {
        foreach (JToken person in rawFeeds[i]["fql_result_set"].Children())
        {
          FacebookUser personObj = FacebookParser.ParseUser(person);
          if (!People.ContainsKey(personObj.ID))
            People.Add(personObj.ID, personObj);
        }
      }

      // Determine current feed status
      FacebookFeedList list = null;
      if (CurrentAction == FeedAction.New)
      {
        Feeds = new ObservableCollection<FacebookFeedList>();
        list = new FacebookFeedList();
      }
      else if (CurrentAction == FeedAction.More)
      {
        list = Feeds[0];
      }

      // Look through all feeds, parsing feeds one by one
      IEnumerable<JToken> rawfeeds = rawFeeds[0]["fql_result_set"].Children();
      foreach (JToken oneResult in rawfeeds)
      {
        FacebookFeed feed = FacebookParser.ParseFeed(oneResult, People);

        // Check if we discarded this feed when parsing by setting it to null
        if (list != null)
          list.Add(feed);
      }

      // Add feeds to our list only if we getting new ones
      //  Else it is already added
      if (CurrentAction == FeedAction.New)
        Feeds.Add(list);

      // Raise feeds parsed event
      OnRaiseFeedsEvent(new SocialLinkEventArgs(CurrentAction));
    }

    private void getQueries(out string q1, out string q2, out string q3, long lastTime = 0)
    {
      q1 = "SELECT post_id, attachment, description, actor_id, target_id, created_time, message," + 
            "comments, likes, message_tags, description, description_tags, type FROM stream " + 
            "WHERE filter_key in (SELECT filter_key FROM stream_filter WHERE uid=me() " +
            "AND type='newsfeed') AND is_hidden = 0 ";
      
      // Fetch more feeds
      if (CurrentAction == FeedAction.More && lastTime > 0)
      {
        q1 += "AND created_time < " + lastTime + " ";
        q1 += "ORDER BY created_time DESC ";
        q1 += "LIMIT " + GeneralSettings.AdditionalFeedsToGet + " ";
      }
      
      // Set limit on refreshing feeds
      if (CurrentAction == FeedAction.New)
      {
        q1 += "ORDER BY created_time DESC ";
        q1 += "LIMIT " + GeneralSettings.InitialFeedsToGet + " ";
      }
      
      q2 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT actor_id FROM #query0)";
      q3 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT target_id FROM #query0)";
    }
  }
}
