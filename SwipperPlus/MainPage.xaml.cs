using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Facebook;
using SwipperPlus.Models;
using SwipperPlus.ViewModels;
using SwipperPlus.Utils;

namespace SwipperPlus
{
  public partial class MainPage : PhoneApplicationPage
  {
    // Constructor
    public MainPage()
    {
      InitializeComponent();
      System.Diagnostics.Debug.WriteLine(SWFacebookUtils.GetAccessToken());
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
      FacebookOAuthClient oauth = new FacebookOAuthClient();
      Uri loginUrl = oauth.GetLoginUrl(SWFacebookUtils.GetLoginParameters());
      System.Diagnostics.Debug.WriteLine(loginUrl.ToString());
      this.authBrowser.Navigate(loginUrl);
    }

    private void authBrowser_Navigating(object sender, NavigatingEventArgs e)
    {
      System.Diagnostics.Debug.WriteLine(e.Uri.ToString());
      FacebookOAuthResult rs;
      if (FacebookOAuthResult.TryParse(e.Uri, out rs)) {
        if (rs.IsSuccess) {
          SWFacebookUtils.SaveAccessToken(rs.AccessToken);
        }else {
          MessageBox.Show("facebook autho failed");
        }
        e.Cancel = true;
        this.authBrowser.Visibility = Visibility.Collapsed;
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
      SWFacebookManager fm = new SWFacebookManager(SWFacebookUtils.GetAccessToken());
      fm.GetNewFeeds();
      fm.FeedsChanged += new EventHandler<SocialLinkEventArgs>(fm_FeedsChanged);
    }

    void fm_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      if (e.Error != null)
        System.Diagnostics.Debug.WriteLine(e.Error.Message);
    }
  }
}