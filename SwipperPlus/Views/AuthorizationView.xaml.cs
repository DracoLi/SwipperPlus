using System;
using System.Windows;
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
using SwipperPlus.Settings;

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

      if (DataContext == null)
        InitializePageState();
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
      DataContext = new ConnectionsManager();
    }

    private void AuthorizeConnection(object sender, RoutedEventArgs e)
    {
      Connection c = (sender as Button).DataContext as Connection;
      string name = c.Name;
      if (name.Equals("Facebook"))
        fb_connection();
      else if (name.Equals("Twitter"))
        tw_connection();
      else if (name.Equals("LinkedIn"))
        li_connection();
    }

    /// <summary>
    /// Open facebook auth page
    /// </summary>
    private void fb_connection()
    {
      // Open facebook auth url
      FacebookOAuthClient oauth = new FacebookOAuthClient();
      Uri loginUrl = oauth.GetLoginUrl(SWFacebookSettings.GetLoginParameters());
      navigateToUri(loginUrl.ToString());
    }

    /// <summary>
    /// Open twitter auth page
    /// </summary>
    private void tw_connection()
    {
      TwitterService twService = new TwitterService(SWTwitterSettings.ConsumerKey, 
        SWTwitterSettings.ConsumerSecret);
      twService.GetRequestToken((token, response) =>
      {
        // Handle errors
        if (token == null)
        {
          Deployment.Current.Dispatcher.BeginInvoke(() => { 
            DisplayError("Cannot request twitter token." + Environment.NewLine + "Please check your internet");
          });
          System.Diagnostics.Debug.WriteLine(response.Response);
          return;
        }
        // Save temp request token for getting access token
        PhoneApplicationService.Current.State[Constants.TW_TOKEN] = token; // Save our twitter token

        // Navigate to auth page
        Uri uri = twService.GetAuthenticationUrl(token);
        Deployment.Current.Dispatcher.BeginInvoke(() =>
        {
          System.Diagnostics.Debug.WriteLine(uri.ToString());
          navigateToUri(uri.ToString());
        });
      });
    }

    /// <summary>
    /// Open linkedin auth page
    /// </summary>
    private void li_connection()
    {
      // Get linkedin authorization url for app
      var authorizer = new OAuthAuthorizer(SWLinkedInSettings.ConsumerKey, SWLinkedInSettings.ConsumerSecret);
      authorizer.GetRequestToken(SWLinkedInSettings.RequestTokenUri)
        .Select(res => res.Token)
        .ObserveOnDispatcher()
        .Subscribe(token =>
        {
          // Save temp request token for getting access token
          PhoneApplicationService.Current.State[Constants.LI_TOKEN] = token;

          //linkedInRequestToken = token;
          string uri = authorizer.BuildAuthorizeUrl(SWLinkedInSettings.AuthorizeUri, token);
          navigateToUri(uri);
        });
    }

    /// <summary>
    /// Displays an error message to the user
    /// </summary>
    /// <param name="errors">A list of errors messages</param>
    /// <param name="caption">The caption</param>
    private void DisplayError(List<string> errors, string caption = null)
    {
      MessageBox.Show(string.Join(Environment.NewLine, errors), caption, MessageBoxButton.OK);
    }

    /// <summary>
    /// Overloaded method for displaying only one error
    /// </summary>
    private void DisplayError(string error, string caption = null)
    {
      List<string> errors = new List<string>();
      errors.Add(error);
      DisplayError(errors, caption);
    }

    /// <summary>
    /// Open our authorization browser to the authorization uri
    /// </summary>
    /// <param name="uri">The authorization uri</param>
    private void navigateToUri(string uri, Dictionary<string, string> pms = null)
    {
      // Construct our url
      string link = "//Views/AuthBrowser.xaml?uri=" + uri;
      if (pms != null)
      {
        foreach (KeyValuePair<string, string> pair in pms)
          link += "&" + pair.Key + "=" + pair.Value;
      }

      // Open our authbrowser passing in only the auth page uri
      NavigationService.Navigate(new Uri(link, UriKind.Relative));
    }
  }
}