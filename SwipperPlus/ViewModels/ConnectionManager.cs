using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SwipperPlus.Model;
using SwipperPlus.Settings;

namespace SwipperPlus.Model
{
  /// <summary>
  /// The connection manager acts as a bridge between the connection data and the settings.
  /// Since the connection models never persists the data, the connection manager must do this
  /// Connection authorization is handled by the view since it needs to interact with the browser
  /// </summary>
  public class ConnectionsManager
  {
    public ObservableCollection<Connection> Connections { get; set; }

    public ConnectionsManager()
    {
      populateConnections();
    }

    private void populateConnections()
    {
      Connection fbfeed = new Connection("Facebook")
      {
        IsConnected = SWFacebookSettings.IsConnected(),
        IsEnabled = SWFacebookSettings.IsEnabled()
      };
      fbfeed.PropertyChanged += new PropertyChangedEventHandler(fbFeed_PropertyChanged);
      Connections.Add(fbfeed);

      Connection twfeed = new Connection("Twitter")
      {
        IsConnected = SWTwitterSettings.IsConnected(),
        IsEnabled = SWTwitterSettings.IsEnabled()
      };
      twfeed.PropertyChanged += new PropertyChangedEventHandler(twfeed_PropertyChanged);
      Connections.Add(twfeed);

      Connection lifeed = new Connection("LinkedIn")
      {
        IsConnected = SWLinkedInSettings.IsConnected(),
        IsEnabled = SWLinkedInSettings.IsEnabled()
      };
      lifeed.PropertyChanged += new PropertyChangedEventHandler(lifeed_PropertyChanged);
      Connections.Add(lifeed);
    }

    private void lifeed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Connection c = sender as Connection;
      if (e.PropertyName.Equals("IsEnabled"))
      {
        SWLinkedInSettings.SetEnabled(c.IsEnabled);
      }
    }

    private void twfeed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Connection c = sender as Connection;
      if (e.PropertyName.Equals("IsEnabled"))
      {
        SWTwitterSettings.SetEnabled(c.IsEnabled);
      }
    }

    private void fbFeed_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Connection c = sender as Connection;
      if (e.PropertyName.Equals("IsEnabled"))
      {
        SWFacebookSettings.SetEnabled(c.IsEnabled);
      }
    }
  }
}
