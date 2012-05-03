using System;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Facebook;
using Newtonsoft.Json.Linq;
using SwipperPlus.Utils;
using SwipperPlus.Utils.Parsers;
using SwipperPlus.Model.Facebook;
using SwipperPlus.Settings;
using SwipperPlus.Views;

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
    string lastRawData;

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
      if (!SWFacebookSettings.IsConnected()) 
        throw new Exception("Social link must be connected!");

      // Set up feeds
      Feeds = new ObservableCollection<FacebookFeedList>();
      FacebookFeedList list = new FacebookFeedList();
      Feeds.Add(list);
      LoadSavedFeeds();

      // Set up others
      People = new Dictionary<ulong, FacebookUser>();
      fb = new FacebookClient(SWFacebookSettings.GetAccessToken());
      fb.GetCompleted += new EventHandler<FacebookApiEventArgs>(fb_GetCompleted);
      IsUpdating = false;
    }

    public override int FeedCount
    {
      get { return Feeds[0].Count; }
    }

    /// <summary>
    /// Get new feeds from facebook and parses the result async
    /// </summary>
    public override void FetchFeeds()
    {
      if (IsUpdating) return;

      // Do this so that we know we are fetching new feeds
      CurrentAction = FeedAction.New;
      string q1, q2, q3;
      getQueries(out q1, out q2, out q3);
      string[] queries = { q1, q2, q3 };
      fb.QueryAsync(queries);
      IsUpdating = true;
    }

    /// <summary>
    /// Get more feeds
    /// </summary>
    /// <param name="amount"></param>
    public override void GetMoreFeeds()
    {
      if (IsUpdating) return;

      // Update new feeds if we have some feeds already
      if (Feeds[0].Count > 0)
      {
        long lastTime = GeneralUtils.DateTimeToUnixTimestamp(Feeds[0][Feeds[0].Count-1].Date);
        CurrentAction = FeedAction.More;
        string q1, q2, q3;
        getQueries(out q1, out q2, out q3, lastTime);
        string[] queries = { q1, q2, q3 };
        fb.QueryAsync(queries);
        IsUpdating = true;
      }
      else
      {
        // If we have no feeds right now, just fetch instead
        FetchFeeds();
      }
    }

    public override void SaveFeeds()
    {
      StorageUtils.SetKeyValue<string>("FacebookFeeds", lastRawData);
    }

    public override void LoadSavedFeeds()
    {
      if (StorageUtils.HasKeyValue("FacebookFeeds"))
        parseRawData(StorageUtils.GetKeyValue<string>("FacebookFeeds"));
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
        IsUpdating = false;
        OnRaiseFeedsEvent(new SocialLinkEventArgs(e.Error.Message));
        return;
      }

      // Parse json results to facebook feeds
      parseRawData(e.GetResultData().ToString());

      // Save data for persistence.
      if (CurrentAction == FeedAction.New)
        lastRawData = e.GetResultData().ToString();
    }

    /// <summary>
    /// Parses facebook json into feeds for this manager
    /// </summary>
    /// <param name="data"></param>
    private void parseRawData(string data)
    {
      JArray rawFeeds = JArray.Parse(data);

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

      // Update our feeds, this effects UI so done in UI thread
      Deployment.Current.Dispatcher.BeginInvoke(() =>
      {
        if (CurrentAction == FeedAction.New)
          Feeds[0].Clear();

        // Look through all feeds, parsing feeds one by one
        IEnumerable<JToken> rawfeeds = rawFeeds[0]["fql_result_set"].Children();
        foreach (JToken oneResult in rawfeeds)
        {
          FacebookFeed feed = FacebookParser.ParseFeed(oneResult, People);

          // Check if we discarded this feed when parsing by setting it to null
          if (feed != null)
            Feeds[0].Add(feed);
        }

        // Raise feeds parsed event
        IsUpdating = false;
        LastUpdated = DateTime.Now;
        OnRaiseFeedsEvent(new SocialLinkEventArgs(CurrentAction));
      });
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
        q1 += "LIMIT 50";
      }
      
      q2 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT actor_id FROM #query0)";
      q3 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT target_id FROM #query0)";
    }
  }
}
