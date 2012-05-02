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

      string type = NavigationContext.QueryString["type"];
      string value = NavigationContext.QueryString["value"];

      if (type == "link")
      {
        dracoBrowser.Navigate(new Uri(value));
      }
    }

    private void authBrowser_Navigating(object sender, NavigatingEventArgs e)
    {
      showLoadingView();
      dracoBrowser.Visibility = Visibility.Visible;
    }

    private void authBrowser_LoadCompleted(object sender, NavigationEventArgs e)
    {
      // Hide loading view when page is loaded
      hideLoadingView();
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