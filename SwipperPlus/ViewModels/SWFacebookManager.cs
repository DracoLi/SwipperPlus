using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using Facebook;
using SwipperPlus.Utils;
using Newtonsoft.Json.Linq;
using SwipperPlus.Models;

namespace SwipperPlus.ViewModels
{
  public class SWFacebookManager : SWSocialLinkManager
  {
    public event EventHandler<SocialLinkEventArgs> FeedsChanged;

    private List<SWFeed> feeds;
    public List<SWFeed> Feeds { set; get; }

    private FacebookClient fb;

    public SWFacebookManager(string accessToken)
    {
      fb = new FacebookClient(accessToken);
      fb.GetCompleted += new EventHandler<FacebookApiEventArgs>(fb_GetCompleted);
    }

    public void GetFeeds()
    {
      
    }

    public void SaveFeeds()
    {

    }

    public void GetSavedFeeds()
    {

    }

    /// <summary>
    /// Get new feeds from facebook and parses the result
    /// </summary>
    public void GetNewFeeds()
    {
      string query0 = "SELECT post_id, attachment, description, actor_id, target_id, created_time, message, comments, likes, message_tags, description, description_tags, type FROM stream WHERE filter_key in (SELECT filter_key FROM stream_filter WHERE uid=me() " +
                        "AND type='newsfeed') AND is_hidden = 0 AND (strlen(message) > 0 OR strlen(description) > 0) ORDER BY created_time DESC LIMIT 0,10";
      string query1 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT actor_id FROM #query0)";
      string query2 = "SELECT id, name, pic_square FROM profile WHERE id IN (SELECT target_id FROM #query0)";
      string[] queries = { query0, query1, query2 };
      fb.QueryAsync(queries);
    }

    void fb_GetCompleted(object sender, FacebookApiEventArgs e)
    {
      if (e.Error != null)
      {
        System.Diagnostics.Debug.WriteLine(e.Error.Message);
        
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
            Person personObj = ParserUtils.ParsePerson(person, SocialLinkType.Facebook);
            if (!People.FBPeople.ContainsKey(personObj.ID))
              People.FBPeople.Add(personObj.ID, personObj);
          }
        }

        // Look through all feeds, parsing it one by one
        IEnumerable<JToken> ienum = rawFeeds[0]["fql_result_set"].Children();
        feeds = new List<SWFeed>();
        foreach (JToken oneResult in ienum)
        {
          feeds.Add(ParserUtils.ParseFeed(oneResult, SocialLinkType.Facebook));
        }

        System.Diagnostics.Debug.WriteLine(People.FBPeople);
      }
      OnRaiseFeedsEvent(new SocialLinkEventArgs(e.Error));
    }

    /// <summary>
    /// Called whenever we want to raise an event
    /// </summary>
    protected virtual void OnRaiseFeedsEvent(SocialLinkEventArgs e)
    {
      if (this.FeedsChanged != null)
      {
        this.FeedsChanged(this, e);
      }
    }
  }
  
  /// <summary>
  /// Fires after new feeds are obtained. If something with feeds, Error property will not be null
  /// </summary>
  public class SocialLinkEventArgs : EventArgs
  {
    private Exception error;
    public Exception Error
    {
      get { return error; }
    }
    public SocialLinkEventArgs(string errorMsg)
    {
      if (!String.IsNullOrEmpty(errorMsg))
      {
        error = new Exception(errorMsg);
      }
    }

    public SocialLinkEventArgs(Exception error)
    {
      this.error = error;
    }
  }
}
