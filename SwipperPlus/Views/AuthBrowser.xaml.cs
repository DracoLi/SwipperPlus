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

      string uri = "";
      if (NavigationContext.QueryString.TryGetValue("uri", out uri))
      {
        this.authBrowser.Navigate(new Uri(uri));
      }
    }

    private void authBrowser_Navigating(object sender, NavigatingEventArgs e)
    {
      showLoadingView();

      #region Facebook authorizatoin
      FacebookOAuthResult rs;
      if (FacebookOAuthResult.TryParse(e.Uri, out rs))
      {
        if (rs.IsSuccess)
        {
          SWFacebookSettings.SetAccessToken(rs.AccessToken);
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

      // TODO: show loading view
    }

    private void authBrowser_LoadCompleted(object sender, NavigationEventArgs e)
    {
      hideLoadingView();
    }

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

      // If have verifier, we get accesstoken
      string verifyPin = results["oauth_verifier"];
      TwitterService twService = new TwitterService(SWTwitterSettings.ConsumerKey,
        SWTwitterSettings.ConsumerSecret);
      OAuthRequestToken twRequestToken = (OAuthRequestToken)PhoneApplicationService.Current.State[Constants.TW_TOKEN];

      // Handle no twitter request token
      if (twRequestToken == null)
      {
        twFailedAndGoBack();
        return;
      }

      // cleanup
      PhoneApplicationService.Current.State.Remove(Constants.TW_TOKEN);

      // Get twitter access token
      twService.GetAccessToken(twRequestToken, verifyPin, (token, response) =>
      {
        if (token == null)
        {
          // Handle when we did not receice a token;
          twFailedAndGoBack();
          return;
        }

        // Now that we got the access token, we can save the tokens
        SWTwitterSettings.SetAccessToken(new AccessToken(token.Token, token.TokenSecret));
        goBack();

        // TODO: Let UI know twitter is authenticated now
      });
    }

    private void handleLinkedInAuthorization(Uri uri)
    {
      var results = GeneralUtils.GetQueryParameters(uri);

      // If no verifier or query, we return to page
      if (results == null || !results.ContainsKey("oauth_verifier"))
      {
        liFailedAndGoBack();
        return;
      }

      string verifyPin = results["oauth_verifier"];
      var authorizer = new OAuthAuthorizer(SWLinkedInSettings.ConsumerKey, SWLinkedInSettings.ConsumerSecret);
      RequestToken liRequestToken = (RequestToken)PhoneApplicationService.Current.State[Constants.LI_TOKEN];

      // Handle no linkedin request token
      if (liRequestToken == null)
      {
        liFailedAndGoBack();
        return;
      }

      // clean up
      PhoneApplicationService.Current.State.Remove(Constants.LI_TOKEN);

      // Set access token
      authorizer.GetAccessToken(SWLinkedInSettings.AccessTokenUri, liRequestToken, verifyPin)
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
          goBack();

          // TODO: notify UI that linkedin is now connected
        });
    }

    /// <summary>
    /// Displays an error message to the user and then go back to last page
    /// </summary>
    /// <param name="errors">A list of errors messages</param>
    /// <param name="caption">The caption</param>
    private void displayErrorAndGoBack(List<string> errors, string caption = null)
    {
      MessageBoxResult r = MessageBox.Show(string.Join(Environment.NewLine, errors), caption, MessageBoxButton.OK);
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
  }
}