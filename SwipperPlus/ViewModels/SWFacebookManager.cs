using System;
using System.Collections.Generic;
using Facebook;
using Newtonsoft.Json.Linq;
using SwipperPlus.Utils;
using SwipperPlus.Utils.Parsers;
using SwipperPlus.Model.Facebook;
using SwipperPlus.Settings;

namespace SwipperPlus.Model
{
  /// <summary>
  /// ViewModel for the Facebook feed
  /// </summary>
  public class SWFacebookManager : SWSocialLinkManager
  {
    /// <summary>
    /// Every facebook feed for the user
    /// </summary>
    public List<FacebookFeed> Feeds { private set; get; }
    
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
      Feeds = new List<FacebookFeed>();

      string q1, q2, q3;
      getQueries(out q1, out q2, out q3);
      string[] queries = { q1, q2, q3 };
      fb.QueryAsync(queries);
    }

    public override void UpdateFeeds()
    {
      long lastTime = GeneralUtils.DateTimeToUnixTimestamp(Feeds[0].Date);
      string q1, q2, q3;
      getQueries(out q1, out q2, out q3, lastTime);
      string[] queries = { q1, q2, q3 };
      fb.QueryAsync(queries);
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

      // Update our people list
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
      FeedStatus status = Feeds.Count == 0 ? FeedStatus.New : FeedStatus.Updated;

      // Look through all feeds, parsing feeds one by one
      IEnumerable<JToken> rawfeeds = rawFeeds[0]["fql_result_set"].Children();
      List<FacebookFeed> tempFeeds = new List<FacebookFeed>();
      foreach (JToken oneResult in rawfeeds)
        tempFeeds.Add(FacebookParser.ParseFeed(oneResult));

      // Add new feeds to top of list
      Feeds.InsertRange(0, tempFeeds);

      // Raise feeds parsed event
      OnRaiseFeedsEvent(new SocialLinkEventArgs(status));
    }

    private void getQueries(out string q1, out string q2, out string q3, long lastTime = 0)
    {
      q1 = "SELECT post_id, attachment, description, actor_id, target_id, created_time, message," + 
            "comments, likes, message_tags, description, description_tags, type FROM stream " + 
            "WHERE filter_key in (SELECT filter_key FROM stream_filter WHERE uid=me() " +
            "AND type='newsfeed') AND is_hidden = 0 ";
      if (lastTime > 0)
        q1 += "AND created_time > " + lastTime + " ";

      q1 += "ORDER BY created_time DESC LIMIT 0,10";
      q2 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT actor_id FROM #query0)";
      q3 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT target_id FROM #query0)";
    }
  }
}
