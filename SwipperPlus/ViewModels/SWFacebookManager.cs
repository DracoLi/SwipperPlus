using System;
using System.Collections.Generic;
using Facebook;
using SwipperPlus.Utils;
using SwipperPlus.Models;
using Newtonsoft.Json.Linq;
using SwipperPlus.Settings;

namespace SwipperPlus.ViewModels
{
  public class SWFacebookManager : SWSocialLinkManager
  {
    public List<SWFacebookFeed> Feeds { private set; get; }

    private FacebookClient fb;

    public SWFacebookManager()
    {
      if (SWFacebookSettings.HasAccessToken())
      {
        fb = new FacebookClient(SWFacebookSettings.GetAccessToken());
        fb.GetCompleted += new EventHandler<FacebookApiEventArgs>(fb_GetCompleted);
      }
    }

    public override bool HasValidAccessToken()
    {
      return fb != null;
    }

    /// <summary>
    /// Get new feeds from facebook and parses the result
    /// </summary>
    public override void FetchFeeds()
    {
      //AND (strlen(message) > 0 OR strlen(description) > 0)
      string query0 = "SELECT post_id, attachment, description, actor_id, target_id, created_time, message, comments, likes, message_tags, description, description_tags, type FROM stream WHERE filter_key in (SELECT filter_key FROM stream_filter WHERE uid=me() " +
                        "AND type='newsfeed') AND is_hidden = 0 ORDER BY created_time DESC LIMIT 0,10";
      string query1 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT actor_id FROM #query0)";
      string query2 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT target_id FROM #query0)";
      string[] queries = { query0, query1, query2 };
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

    void fb_GetCompleted(object sender, FacebookApiEventArgs e)
    {
      if (e.Error != null)
      {
        System.Diagnostics.Debug.WriteLine(e.Error.Message);
        OnRaiseFeedsEvent(new SocialLinkEventArgs(e.Error.Message));
        return;
      }
      else
      {
        // Parse json result to feeds
        System.Diagnostics.Debug.WriteLine(e.GetResultData().ToString());
        JArray rawFeeds = JArray.Parse(e.GetResultData().ToString());

        // Update our people list
        for (int i = 1; i < 3; i++)
        {
          foreach (JToken person in rawFeeds[i]["fql_result_set"].Children())
          {
            FacebookUser personObj = FacebookParser.ParseUser(person);
            if (!People.FBPeople.ContainsKey(personObj.ID))
              People.FBPeople.Add(personObj.ID, personObj);
          }
        }

        // Look through all feeds, parsing it one by one
        IEnumerable<JToken> ienum = rawFeeds[0]["fql_result_set"].Children();
        Feeds = new List<SWFacebookFeed>();
        foreach (JToken oneResult in ienum)
          Feeds.Add(FacebookParser.ParseFeed(oneResult));

        System.Diagnostics.Debug.WriteLine(People.FBPeople);

        OnRaiseFeedsEvent(new SocialLinkEventArgs(null));
      }
    }
  }
}
