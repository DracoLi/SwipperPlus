using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace SwipperPlus.Views
{
  public partial class SWBrowser : PhoneApplicationPage
  {
    public SWBrowser()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      System.Diagnostics.Debug.WriteLine(e.Uri.ToString());
      string type = NavigationContext.QueryString["type"];
      string value = NavigationContext.QueryString["value"];

      // This is a link and no site has been loaded yet
      if (type == "link" && dracoBrowser.Visibility == Visibility.Collapsed)
      {
        dracoBrowser.Navigate(new Uri(value));
      }
    }

    private void authBrowser_Navigating(object sender, NavigatingEventArgs e)
    {
      showLoadingBar();
      dracoBrowser.Visibility = Visibility.Visible;
    }

    private void authBrowser_LoadCompleted(object sender, NavigationEventArgs e)
    {
      // Hide loading view when page is loaded
      hideLoadingBar();
    }

    /// <summary>
    /// Display a loading view
    /// </summary>
    public void showLoadingBar()
    {
      loadingView.Visibility = Visibility.Visible;
      progressBar.IsIndeterminate = true;
    }

    /// <summary>
    /// Hide the current loading view
    /// </summary>
    public void hideLoadingBar()
    {
      loadingView.Visibility = Visibility.Collapsed;
      progressBar.IsIndeterminate = false;
    }
  }
}