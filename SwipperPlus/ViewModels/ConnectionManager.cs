using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SwipperPlus.Model;
using SwipperPlus.Settings;

namespace SwipperPlus.ViewModel
{
  /// <summary>
  /// The connection manager acts as a bridge between the connection data and the settings.
  /// Since the connection models never persists the data, the connection manager must do this
  /// Connection authorization is handled by the view since it needs to interact with the browser
  /// </summary>
  public class ConnectionsManager
  {
    public ObservableCollection<SWConnection> Connections { get; set; }

    public ConnectionsManager()
    {
      Connections = new ObservableCollection<SWConnection>();
      populateConnections();
    }

    private void populateConnections()
    {
      SWConnection fbfeed = new SWConnection("Facebook")
      {
        IsConnected = SWFacebookSettings.IsConnected(),
      };
      fbfeed.PropertyChanged += new PropertyChangedEventHandler(fbFeed_PropertyChanged);
      Connections.Add(fbfeed);

      SWConnection twfeed = new SWConnection("Twitter")
      {
        IsConnected = SWTwitterSettings.IsConnected(),
      };
      twfeed.PropertyChanged += new PropertyChangedEventHandler(twfeed_PropertyChanged);
      Connections.Add(twfeed);

      SWConnection lifeed = new SWConnection("LinkedIn")
      {
        IsConnected = SWLinkedInSettings.IsConnected(),
      };
      lifeed.PropertyChanged += new PropertyChangedEventHandler(lifeed_PropertyChanged);
      Connections.Add(lifeed);
    }

    private void lifeed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      SWConnection c = sender as SWConnection;
      if (e.PropertyName.Equals("IsConnected") && !c.IsConnected)
      {
        SWLinkedInSettings.RemoveAccessToken();
      }
    }

    private void twfeed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      SWConnection c = sender as SWConnection;
      if (e.PropertyName.Equals("IsConnected") && !c.IsConnected)
      {
        SWTwitterSettings.RemoveAccessToken();
      }
    }

    private void fbFeed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      SWConnection c = sender as SWConnection;
      if (e.PropertyName.Equals("IsConnected") && !c.IsConnected)
      {
        SWFacebookSettings.RemoveAccessToken();
      }
    }
  }
}
