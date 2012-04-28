using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Reactive;
using Facebook;
using TweetSharp;
using Codeplex.OAuth;
using SwipperPlus.Settings;
using SwipperPlus.Utils;
using SwipperPlus.Model;

namespace SwipperPlus.Views
{
  /// <summary>
  /// Handles the authorization of different api's we support
  /// </summary>
  public partial class AuthBrowser : PhoneApplicationPage
  {
    public AuthBrowser()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      // Hide auth browser until we are loading
      authBrowser.Visibility = Visibility.Collapsed;
      
      // Figure out what we need to authorize
      if (NavigationContext.QueryString.ContainsKey(Constants.AUTH_LINK))
      {
        showLoadingView();
        string type = NavigationContext.QueryString[Constants.AUTH_LINK];

        // We need internet connect from everything here. Check that please!
        if (!GeneralUtils.HasInternetConnection())
        {
          displayErrorAndGoBack(type +" cannot be authorized." + Environment.NewLine +
            "Please check you internet connection and try again", "No Internet Connection");
          return;
        }

        // Handle social link authorization
        if (type.Equals(Constants.FACEBOOK))
          fb_connection();
        else if (type.Equals(Constants.TWITTER))
          tw_connection();
        else if (type.Equals(Constants.LINKEDIN))
          li_connection();
        else
          showGenericErrorAndGoBack();
      }
      else
      {
        showGenericErrorAndGoBack();
      }
    }
 
    #region Prepare authorization page

    // Temp vars to store request tokens
    private OAuthRequestToken tw_requestToken;
    private RequestToken      li_requestToken;

    /// <summary>
    /// Open facebook auth page
    /// </summary>
    private void fb_connection()
    {
      // Open facebook auth url
      FacebookOAuthClient oauth = new FacebookOAuthClient();
      Uri loginUrl = oauth.GetLoginUrl(SWFacebookSettings.GetLoginParameters());
      authBrowser.Navigate(loginUrl);
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
          Deployment.Current.Dispatcher.BeginInvoke(() =>
          {
            displayErrorAndGoBack("Cannot authorize Twitter." + Environment.NewLine 
              + "Please check your internet");
          });
          System.Diagnostics.Debug.WriteLine(response.Response);
          return;
        }

        // Save request token
        this.tw_requestToken = token;

        // Navigate to auth page
        Uri uri = twService.GetAuthenticationUrl(token);
        Deployment.Current.Dispatcher.BeginInvoke(() =>
        {
          System.Diagnostics.Debug.WriteLine(uri.ToString());
          authBrowser.Navigate(uri);
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
        // Save request token
        this.li_requestToken = token;

        //linkedInRequestToken = token;
        string uri = authorizer.BuildAuthorizeUrl(SWLinkedInSettings.AuthorizeUri, token);
        authBrowser.Navigate(new Uri(uri));
      });
    }

    #endregion

    private void authBrowser_Navigating(object sender, NavigatingEventArgs e)
    {
      showLoadingView();
      authBrowser.Visibility = Visibility.Visible;

      #region Facebook authorization
      FacebookOAuthResult rs;
      if (FacebookOAuthResult.TryParse(e.Uri, out rs))
      {
        if (rs.IsSuccess)
        {
          SWFacebookSettings.SetAccessToken(rs.AccessToken);
          System.Diagnostics.Debug.WriteLine("Facebook access token: " + rs.AccessToken);
          PhoneApplicationService.Current.State[Constants.AUTH_NOTIFICATION] = "Facebook is now enabled!";
          goBack();
          // TODO: Notify UI facebook is authenticated now
        }
        else
        {
          displayErrorAndGoBack("Facebook cannot be authenticated right now." + Environment.NewLine
            + "Please try again later");
        }
        e.Cancel = true;
        return;
      }
      #endregion

      // Twitter
      if (SWTwitterSettings.IsTwitterCallback(e.Uri))
      {
        handleTwitterAuthorization(e.Uri);
        e.Cancel = true;
        return;
      }

      // LinkedIn
      if (SWLinkedInSettings.IsLinkedInCallback(e.Uri))
      {
        handleLinkedInAuthorization(e.Uri);
        e.Cancel = true;
        return;
      }
    }

    private void authBrowser_LoadCompleted(object sender, NavigationEventArgs e)
    {
      // Hide loading view when page is loaded
      hideLoadingView();
    }

    #region Page authorization (Access Tokens)
    
    /// <summary>
    /// Get Twitter's access token from auth uri
    /// </summary>
    private void handleTwitterAuthorization(Uri uri)
    {
      // Get the verifier from the redirected url and get access token
      var results = GeneralUtils.GetQueryParameters(uri);

      // Handle when we don't have a twitter verfier
      if (results == null || !results.ContainsKey("oauth_verifier"))
      {
        twFailedAndGoBack();
        return;
      }

      // Handle no twitter request token
      if (this.tw_requestToken == null)
      {
        twFailedAndGoBack();
        return;
      }

      // Get twitter access token
      string verifyPin = results["oauth_verifier"];
      TwitterService twService = new TwitterService(SWTwitterSettings.ConsumerKey,
        SWTwitterSettings.ConsumerSecret);
      twService.GetAccessToken(this.tw_requestToken, verifyPin, (token, response) =>
      {
        if (token == null)
        {
          // Handle when we did not receice a token;
          Deployment.Current.Dispatcher.BeginInvoke(() => { twFailedAndGoBack(); });
          return;
        }

        // Now that we got the access token, we can save the tokens
        SWTwitterSettings.SetAccessToken(new AccessToken(token.Token, token.TokenSecret));
        System.Diagnostics.Debug.WriteLine("Twitter access token: " + token.Token);
        PhoneApplicationService.Current.State[Constants.AUTH_NOTIFICATION] = "Twitter is now enabled!";
        Deployment.Current.Dispatcher.BeginInvoke(() => { goBack(); });

        // TODO: Let UI know twitter is authenticated now
      });
    }

    /// <summary>
    /// Get linkedin access token from uri
    /// </summary>
    private void handleLinkedInAuthorization(Uri uri)
    {
      var results = GeneralUtils.GetQueryParameters(uri);

      // If no verifier or query, we return to page
      if (results == null || !results.ContainsKey("oauth_verifier"))
      {
        liFailedAndGoBack();
        return;
      }

      // Handle no linkedin request token
      if (li_requestToken == null)
      {
        liFailedAndGoBack();
        return;
      }

      // clean up
      StorageUtils.RemoveData(Constants.LI_TOKEN);

      // Get access token
      string verifyPin = results["oauth_verifier"];
      var authorizer = new OAuthAuthorizer(SWLinkedInSettings.ConsumerKey, SWLinkedInSettings.ConsumerSecret);
      authorizer.GetAccessToken(SWLinkedInSettings.AccessTokenUri, this.li_requestToken, verifyPin)
        .ObserveOnDispatcher()
        .Subscribe(res =>
        {
          if (res == null)
          {
            liFailedAndGoBack();
            return;
          }
          SWLinkedInSettings.SetAccessToken(res.Token);
          System.Diagnostics.Debug.WriteLine("Successfully got linkedin access token: " + res.Token.ToString());
          PhoneApplicationService.Current.State[Constants.AUTH_NOTIFICATION] = "LinkedIn is now enabled!";
          goBack();

          // TODO: notify UI that linkedin is now connected
        });
    }

    #endregion

    #region UI Actions (goback, display messages, loading etc)
    
    /// <summary>
    /// Displays an error message to the user and then go back to last page
    /// </summary>
    /// <param name="errors">A list of errors messages</param>
    /// <param name="caption">The caption</param>
    private void displayErrorAndGoBack(List<string> errors, string caption = null)
    {
      hideLoadingView();
      MessageBoxResult r;
      if (caption != null)
        r = MessageBox.Show(string.Join(Environment.NewLine, errors), caption, MessageBoxButton.OK);
      else
        r = MessageBox.Show(string.Join(Environment.NewLine, errors));

      if (r == MessageBoxResult.OK)
        goBack();
    }

    /// <summary>
    /// Overloaded method for displaying only one error
    /// </summary>
    private void displayErrorAndGoBack(string error, string caption = null)
    {
      List<string> errors = new List<string>();
      errors.Add(error);
      displayErrorAndGoBack(errors, caption);
    }

    private void showGenericErrorAndGoBack()
    {
      displayErrorAndGoBack("Error Reason Unknown." + Environment.NewLine + 
        "Please try again later.", "Authorization Failed");
    }

    /// <summary>
    /// Quick helper to display that twitter failed
    /// </summary>
    private void twFailedAndGoBack()
    {
      displayErrorAndGoBack("Twitter authentication failed." + Environment.NewLine +
        "Please go back and try again.");
    }

    /// <summary>
    /// Quick helper to display that linkedin failed
    /// </summary>
    private void liFailedAndGoBack()
    {
      displayErrorAndGoBack("LinkedIn authentication failed." + Environment.NewLine +
        "Please go back and try again.");
    }

    private void goBack()
    {
      NavigationService.GoBack();
    }

    /// <summary>
    /// Display a loading view
    /// </summary>
    public void showLoadingView()
    {
      loadingView.Visibility = Visibility.Visible;
      progressBar.IsIndeterminate = true;
    }

    /// <summary>
    /// Hide the current loading view
    /// </summary>
    public void hideLoadingView()
    {
      loadingView.Visibility = Visibility.Collapsed;
      progressBar.IsIndeterminate = false;
    }

    #endregion
  }
}