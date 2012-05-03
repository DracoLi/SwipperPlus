using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using SwipperPlus.Utils;
using SwipperPlus.ViewModel;
using SwipperPlus.Settings;
using SwipperPlus.Model.Facebook;

namespace SwipperPlus.Views
{
  public partial class FBPivotView : UserControl
  {
    EventHandler<SocialLinkEventArgs> loadingHandler;
    DateTime lastUpdate;

    public FBPivotView()
    {
      InitializeComponent();

      loadingHandler = new EventHandler<SocialLinkEventArgs>(manager_FeedsChanged);
    }

    #region Feed Clicking Events

    /// <summary>
    /// Handle attachment link click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Attachment_Click(object sender, RoutedEventArgs e)
    {
      Button attachmentButton = sender as Button;
      System.Diagnostics.Debug.WriteLine(attachmentButton.Tag.ToString());

      // Navigate to uri
      string link = attachmentButton.Tag.ToString();
      GetMainPage().NavigationService.Navigate(GeneralUtils.UriForWebsiteNavigation(link));
    }

    /// <summary>
    /// Handle attachment image click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ImageAttachment_Click(object sender, RoutedEventArgs e)
    {
      // Navigate to uri
      Button attachmentButton = sender as Button;
      string link = attachmentButton.Tag.ToString();
      GetMainPage().NavigationService.Navigate(GeneralUtils.UriForImageViewing(link));
    }

    #endregion

    #region Updating Feeds related methods

    /// <summary>
    /// Fix the jumpy listselector bug
    /// </summary>
    private void LongListSelector_ScrollingStarted(object sender, EventArgs e)
    {
      GetMainPage().Focus();
    }

    private void LongListSelector_StretchingBottom(object sender, EventArgs e)
    {
      SWFacebookManager manager = (this.DataContext as SWFacebookManager);

      if (manager.IsUpdating ||
        (DateTime.Now - lastUpdate).TotalMilliseconds < GeneralSettings.FetchDelay)
      {
        System.Diagnostics.Debug.WriteLine("Update ignored");
        return;
      }

      // Make sure our loading handler is attached
      manager.FeedsChanged -= loadingHandler;
      manager.FeedsChanged += loadingHandler;

      // Get feeds
      manager.GetMoreFeeds();
      ShowLoadingBar("Loading More Facebook Feeds..");
    }


    void manager_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      HideLoadingBar();
      lastUpdate = DateTime.Now;
    }

    #endregion

    #region Loading Event Helpers

    private void ShowLoadingBar(string msg)
    {
      Deployment.Current.Dispatcher.BeginInvoke(() =>
      {
        GetMainPage().ShowLoadingBar(msg);
      });
    }

    private void HideLoadingBar()
    {
      Deployment.Current.Dispatcher.BeginInvoke(() =>
      {
        GetMainPage().HideLoadingBar();
      });
    }

    #endregion

    #region Helpers

    private MainPage GetMainPage()
    {
      return (MainPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content; 
    }

    #endregion
  }
}
