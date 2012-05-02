using System;
using System.Collections.ObjectModel;

namespace SwipperPlus.Model.Facebook
{
  public class FacebookFeedList : ObservableCollection<FacebookFeed>
  {
    public void AddFeed(FacebookFeed feed)
    {
      Items.Add(feed);
    }

    public FacebookFeedList_SerializationWrapper ToWrapper()
    {
      return new FacebookFeedList_SerializationWrapper() { Feeds = this.Items };
    }

    public class FacebookFeedList_SerializationWrapper
    {
      public string Name { get; set; }
      public System.Collections.Generic.IList<FacebookFeed> Feeds { get; set; }

      public FacebookFeedList Unwrap()
      {
        FacebookFeedList feedList = new FacebookFeedList();
        foreach (FacebookFeed feed in this.Feeds)
        {
          feedList.AddFeed(feed);
        }
        return feedList;
      }
    }
  }
}
