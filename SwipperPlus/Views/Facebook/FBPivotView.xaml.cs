using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Phone.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using SwipperPlus.Utils;

namespace SwipperPlus.Views.Facebook
{
  public partial class FBPivotView : UserControl
  {
    public FBPivotView()
    {
      InitializeComponent();
    }

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
      PhoneApplicationPage currentPage = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content;
      string link = attachmentButton.Tag.ToString();
      currentPage.NavigationService.Navigate(GeneralUtils.UriForWebsiteNavigation(link));
    }

    /// <summary>
    /// Handle attachment image click
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ImageAttachment_Click(object sender, RoutedEventArgs e)
    {
      // Navigate to uri
      PhoneApplicationPage currentPage = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content;
      Button attachmentButton = sender as Button;
      string link = attachmentButton.Tag.ToString();
      currentPage.NavigationService.Navigate(GeneralUtils.UriForImageViewing(link));
    }

    /// <summary>
    /// Fix the jumpy listselector bug
    /// </summary>
    private void LongListSelector_ScrollingStarted(object sender, EventArgs e)
    {
      PhoneApplicationPage currentPage = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content;
      currentPage.Focus();
    }
  }
}
