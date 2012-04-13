using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Facebook;
using SwipperPlus.Models;
using SwipperPlus.ViewModels;
using SwipperPlus.Settings;
using TweetSharp;
using SwipperPlus.Utils;
using Codeplex.OAuth;
using Microsoft.Phone.Reactive;

namespace SwipperPlus
{
  public partial class MainPage : PhoneApplicationPage
  {

    private TwitterService twService;
    private OAuthRequestToken twRequestToken;
    private SWFacebookManager facebookManager;
    private SWTwitterManager twitterManager;
    private RequestToken linkedInRequestToken;

    // Constructor
    public MainPage()
    {
      InitializeComponent();
      System.Diagnostics.Debug.WriteLine(SWFacebookSettings.GetAccessToken());
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      FacebookOAuthClient oauth = new FacebookOAuthClient();
      Uri loginUrl = oauth.GetLoginUrl(SWFacebookSettings.GetLoginParameters());
      System.Diagnostics.Debug.WriteLine(loginUrl.ToString());
      this.authBrowser.Navigate(loginUrl);
    }

    private void authBrowser_Navigating(object sender, NavigatingEventArgs e)
    {
      System.Diagnostics.Debug.WriteLine(e.Uri.ToString());
      FacebookOAuthResult rs;
      if (FacebookOAuthResult.TryParse(e.Uri, out rs)) {
        if (rs.IsSuccess) {
          SWFacebookSettings.SaveAccessToken(rs.AccessToken);
        }else {
          MessageBox.Show("facebook autho failed");
        }
        e.Cancel = true;
        this.authBrowser.Visibility = Visibility.Collapsed;
      }

      if (SWTwitterSettings.IsTwitterCallback(e.Uri))
      {
        HandleTwitterAuthorization(e.Uri);
        e.Cancel = true;
      }
      else if (SWLinkedInSettings.IsLinkedInCallback(e.Uri))
      {
        HandleLinkedInAuthorization(e.Uri);
        e.Cancel = true;
      }
    }

    private void authBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
      authBrowser.Visibility = Visibility.Visible;
    }

    private void authBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
    {
     
    }

    private void button2_Click(object sender, RoutedEventArgs e)
    {
      if (facebookManager == null)
      {
        facebookManager = new SWFacebookManager();
        facebookManager.FeedsChanged += new EventHandler<SocialLinkEventArgs>(fm_FeedsChanged);
      }
        
      if (facebookManager.HasValidAccessToken())
        facebookManager.FetchFeeds();
    }

    void fm_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      if (e.Error != null)
        System.Diagnostics.Debug.WriteLine(e.Error.Message);
    }

    private void button3_Click(object sender, RoutedEventArgs e)
    {
      if (twService == null)
        twService = new TwitterService(SWTwitterSettings.ConsumerKey, SWTwitterSettings.ConsumerSecret);
      twService.GetRequestToken((token, response) =>
        {
          if (token == null)
          {
            Deployment.Current.Dispatcher.BeginInvoke(() => { MessageBox.Show("Cannot request twitter"); });
            System.Diagnostics.Debug.WriteLine(response.Response);
            return;
          }
          this.twRequestToken = token;
          Uri uri = twService.GetAuthenticationUrl(token);
          Deployment.Current.Dispatcher.BeginInvoke(()=> {
            System.Diagnostics.Debug.WriteLine(uri.ToString());
            this.authBrowser.Navigate(uri);
          });
        }
      );
    }

    private void button4_Click(object sender, RoutedEventArgs e)
    {
      if (twitterManager == null)
      {
        twitterManager = new SWTwitterManager();
        twitterManager.FeedsChanged += new EventHandler<SocialLinkEventArgs>(st_FeedsChanged);
      }
        
      if (twitterManager.HasValidAccessToken())
        twitterManager.FetchFeeds();
    }

    void st_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      if (e.Error != null)
        System.Diagnostics.Debug.WriteLine(e.Error.Message);
    }

    private void button5_Click(object sender, RoutedEventArgs e)
    {
      // Validate linkedin
      var authorizer = new OAuthAuthorizer(SWLinkedInSettings.ConsumerKey, SWLinkedInSettings.ConsumerSecret);
      authorizer.GetRequestToken(SWLinkedInSettings.RequestTokenUri)
        .Select(res => res.Token)
        .ObserveOnDispatcher()
        .Subscribe(token =>
        {
          linkedInRequestToken = token;
          string url = authorizer.BuildAuthorizeUrl(SWLinkedInSettings.AuthorizeUri, token);
          this.authBrowser.Navigate(new Uri(url));
        });
    }

    private void button6_Click(object sender, RoutedEventArgs e)
    {
      OAuthClient client = new OAuthClient(SWLinkedInSettings.ConsumerKey, SWLinkedInSettings.ConsumerSecret, SWLinkedInSettings.GetAccessToken())
      {
        Url = "http://api.linkedin.com/v1/people/~/network/updates",
        Parameters = { { "format", "json" } }
      };
      client.GetResponseText()
        .Subscribe(a =>
        {
          System.Diagnostics.Debug.WriteLine(a);
        });
    }

    private void HandleTwitterAuthorization(Uri url)
    {
      // Get the verifier from the redirected url and get access token
      var results= GeneralUtils.GetQueryParameters(url);

      // If no verifier, we return to page
      if (results == null || !results.ContainsKey("oauth_verifier"))
      {
        MessageBox.Show("Reason unknown", "Twitter Authentication Failed", MessageBoxButton.OK);
        return;
      }

      // If have verifier, we get accesstoken
      string verifyPin = results["oauth_verifier"];
      twService.GetAccessToken(twRequestToken, verifyPin, (token, response) =>
      {
        if (token == null)
        {
          // Handle when we did not receice a token;
          System.Diagnostics.Debug.WriteLine("cannot obtain twitter token!");
          return;
        }
        else
        {
          // Now that we got the access token, we can save the tokens
          SWTwitterSettings.SetAccessToken(token);

          // Notify UI we can twitter feeds now
        }

        // Remove browser
        Deployment.Current.Dispatcher.BeginInvoke(() =>
        {
          this.authBrowser.Visibility = Visibility.Collapsed;
        });
      });
    }

    private void HandleLinkedInAuthorization(Uri url)
    {
      var results = GeneralUtils.GetQueryParameters(url);
      
      // If no verifier or query, we return to page
      if (results == null || !results.ContainsKey("oauth_verifier"))
      {
        MessageBox.Show("Reason unknown", "Twitter Authentication Failed", MessageBoxButton.OK);
        return;
      }

      string verifyPin = results["oauth_verifier"];
      var authorizer = new OAuthAuthorizer(SWLinkedInSettings.ConsumerKey, SWLinkedInSettings.ConsumerSecret);
      authorizer.GetAccessToken(SWLinkedInSettings.AccessTokenUri, linkedInRequestToken, verifyPin)
        .ObserveOnDispatcher()
        .Subscribe(res =>
        {
          SWLinkedInSettings.SetAccessToken(res.Token);
          System.Diagnostics.Debug.WriteLine("Successfully got linkedin access token: " + res.Token.ToString());
        });
    }
  }
}