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
using Codeplex.OAuth;
using Microsoft.Phone.Reactive;
using Facebook;
using TweetSharp;
using SwipperPlus.ViewModel;
using SwipperPlus.Settings;
using SwipperPlus.Utils;

namespace SwipperPlus
{
  public partial class MainPage : PhoneApplicationPage
  {
    SWTwitterManager twManager;
    SWLinkedInManager liManager;
    SWFacebookManager fbManager;

    public MainPage()
    {
      InitializeComponent();

      if (SWTwitterSettings.IsConnected())
        twManager = new SWTwitterManager();
      if (SWFacebookSettings.IsConnected())
        fbManager = new SWFacebookManager();
      if (SWLinkedInSettings.IsConnected())
        liManager = new SWLinkedInManager();

      IntializePage();
    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
 	    base.OnNavigatedTo(e);
      if (SWFacebookSettings.IsConnected())
      {
        fbManager = new SWFacebookManager();
        fbManager.FeedsChanged += new EventHandler<SocialLinkEventArgs>(fbManager_FeedsChanged);
        fbManager.FetchFeeds();
      }
    }
    void fbManager_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      Deployment.Current.Dispatcher.BeginInvoke(() => { TwitterView.DataContext = fbManager; });
    }
  
    void IntializePage()
    {
      /*
      if (fbManager != null)
      {
        PivotItem fbItem = new PivotItem();
        fbItem.DataContext = fbManager;
      }
      */

      if (twManager != null)
      {
        PivotItem twItem = new PivotItem();
        twItem.DataContext = twManager;
      }
    }

    void ShowAuthorizationView(object sender, EventArgs e)
    {
      this.NavigationService.Navigate(new Uri("/Views/AuthorizationView.xaml", UriKind.Relative));
    }

    void ShowSettingsView(object sender, EventArgs e)
    {

    }
  }
}