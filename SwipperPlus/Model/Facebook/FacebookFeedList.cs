using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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
