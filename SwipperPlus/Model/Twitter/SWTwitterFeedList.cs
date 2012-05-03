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

namespace SwipperPlus.Model.Twitter
{
  public class SWTwitterFeedList : ObservableCollection<SWTwitterFeed>
  {
    public void AddFeed(SWTwitterFeed feed)
    {
      Items.Add(feed);
    }

    public SWTwitterFeedList_SerializationWrapper ToWrapper()
    {
      return new SWTwitterFeedList_SerializationWrapper() { Feeds = this.Items };
    }

    public class SWTwitterFeedList_SerializationWrapper
    {
      public string Name { get; set; }
      public System.Collections.Generic.IList<SWTwitterFeed> Feeds { get; set; }

      public SWTwitterFeedList Unwrap()
      {
        SWTwitterFeedList feedList = new SWTwitterFeedList();
        foreach (SWTwitterFeed feed in this.Feeds)
        {
          feedList.AddFeed(feed);
        }
        return feedList;
      }
    }
  }
}
