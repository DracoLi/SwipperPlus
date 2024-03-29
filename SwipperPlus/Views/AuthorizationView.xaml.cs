﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Web;
using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using Microsoft.Phone.Reactive;
using System.Collections.Generic;
using Facebook;
using TweetSharp;
using Codeplex.OAuth;
using SwipperPlus.Model;
using SwipperPlus.ViewModel;

namespace SwipperPlus.Views
{
  public partial class AuthorizationView : PhoneApplicationPage
  {
    public AuthorizationView()
    {
      InitializeComponent();
    }
    
    /// <summary>
    /// Called when navigating to this page; Loads all the connections
    /// on the first navigation (that is, at application launch and
    /// reactivation) and initializes the page state.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      // Refresh data context since user could have authorized something
      InitializePageState();

      // Check if we just enabled a link, if so display notification
      if (PhoneApplicationService.Current.State.ContainsKey(Constants.AUTH_NOTIFICATION))
      {
        string notification = (string)PhoneApplicationService.Current.State[Constants.AUTH_NOTIFICATION];
        PhoneApplicationService.Current.State.Remove(Constants.AUTH_NOTIFICATION);
        showNotification(notification);
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);
    }

    /// <summary>
    /// Initializes the view. 
    /// </summary>
    private void InitializePageState()
    {
      // Update all our social link content
      DataContext = new ConnectionsManager();
    }

    #region Authentication/Deauthentication
    
    /// <summary>
    /// Handles the authorization and deauthorization of links
    /// </summary>
    private void ConnectButtonPressed(object sender, RoutedEventArgs e)
    {
      // Handle this command
      SWConnection c = (sender as Button).DataContext as SWConnection;
      if (c.IsConnected)
      {
        // Deauthorize the connection if user confirms
        string name = c.Name;
        MessageBoxResult r = MessageBox.Show("Remove " + name + " from the application?", 
          "Disconnect " + name, MessageBoxButton.OKCancel);
        if (r == MessageBoxResult.OK)
        {
          deauthorizeConnection(c);
        }
      }
      else
      {
        authorizeConnection(c);
      }
    }
    
    /// <summary>
    /// Authorize a connection
    /// </summary>
    private void authorizeConnection(SWConnection c)
    {
      // Authorize this connection
      authenticateLink(c.Name);
    }

    /// <summary>
    /// Deauthorize a connection
    /// </summary>
    private void deauthorizeConnection(SWConnection c)
    {
      // Manually set isconnected to false to update UI and settings
      c.IsConnected = false;
    }

    /// <summary>
    /// Open our authorization browser for the requested link
    /// </summary>
    /// <param name="type">The type of the authentication</param>
    private void authenticateLink(string type)
    {
      // Open our authbrowser passing in only the social link we need to authorize
      string uri = "//Views/AuthBrowser.xaml?" + Constants.AUTH_LINK + "=" + type;
      NavigationService.Navigate(new Uri(uri, UriKind.Relative));
    }
    
    #endregion

    #region Notifications and Errors

    /// <summary>
    /// Displays the notification panel with supplied text
    /// </summary>
    /// <param name="text">Text to display</param>
    private void showNotification(string text, double sec = 1)
    {
      NotificationText.Text = text;
      NotificationAnimateInOut.Begin();
    }

    /// <summary>
    /// Displays an error message to the user
    /// </summary>
    /// <param name="errors">A list of errors messages</param>
    /// <param name="caption">The caption</param>
    private void displayError(List<string> errors, string caption = null)
    {
      if (caption != null)
        MessageBox.Show(string.Join(Environment.NewLine, errors), caption, MessageBoxButton.OK);
      else
        MessageBox.Show(string.Join(Environment.NewLine, errors));
    }

    /// <summary>
    /// Overloaded method for displaying only one error
    /// </summary>
    private void displayError(string error, string caption = null)
    {
      List<string> errors = new List<string>();
      errors.Add(error);
      displayError(errors, caption);
    }

    #endregion
  }
}