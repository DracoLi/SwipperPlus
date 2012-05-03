using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Facebook;
using TweetSharp;
using SwipperPlus.ViewModel;
using SwipperPlus.Settings;
using SwipperPlus.Utils;
using SwipperPlus.Views;

namespace SwipperPlus
{
  public partial class MainPage : PhoneApplicationPage
  {
    public List<SWSocialLinkManager> Managers { get; set; }
    public List<UserControl> PivotContents { get; set; }

    private ApplicationBarMenuItem refreshButton;

    // Constants
    const string PivotIndexKey = "pivotindex";

    public MainPage()
    {
      InitializeComponent();

      Managers = new List<SWSocialLinkManager>();
      PivotContents = new List<UserControl>();
      refreshButton = ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
      IntializePage();
    }

    void IntializePage()
    {

    }

    protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
    {
 	    base.OnNavigatedTo(e);

      // Create all our pivot manager for currently connected feeds
      int newFeeds = 0;
      // Add Facebook
      if (SWFacebookSettings.IsConnected() && getFacebookManager() == null)
      {
        SWFacebookManager fbManager = new SWFacebookManager();
        fbManager.FeedsChanged += new EventHandler<SocialLinkEventArgs>(fbManager_FeedsChanged);
        Managers.Add(fbManager);
        addPivotForManager(fbManager, new FBPivotView());
        newFeeds++;
      }

      // Add Twitter
      if (SWTwitterSettings.IsConnected() && getTwitterManager() == null)
      {
        SWTwitterManager twManager = new SWTwitterManager();
        twManager.FeedsChanged += new EventHandler<SocialLinkEventArgs>(twitter_FeedsChanged);
        Managers.Add(twManager);
        addPivotForManager(twManager, new TWPivotView());
        newFeeds++;
      }
      
      // Determine which feeds to show
      int currentIndex = 0; // Show first feed by default
      if (newFeeds > 0)
      {
        // Show the first added feed
        currentIndex = Managers.Count - newFeeds;
      }
      else if (newFeeds == 0 && State.ContainsKey(PivotIndexKey))
      {
        // Show our saved index if nothing is added
        if (State.ContainsKey(PivotIndexKey))
          currentIndex = (int)State[PivotIndexKey];
      }
      // Show determined index
      MainPivot.SelectedIndex = currentIndex;

      // Refresh current index (if we can and lastFresh is over our limit)
      if (Managers.Count > 0 && 
          currentIndex <= Managers.Count - 1)
      {
        SWSocialLinkManager manager = Managers[currentIndex];
        if (manager.LastUpdated == null ||
          (DateTime.Now - manager.LastUpdated).TotalMilliseconds > GeneralSettings.RefreshDelay)
          FetchFeedsForManager(currentIndex);
      }

      // Enable/Disable refresh button
      if (PivotContents.Count == 0) refreshButton.IsEnabled = false;
      else refreshButton.IsEnabled = true;
    }

    protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);
      
      // Save current index
      if (Managers.Count > 0)
        State[PivotIndexKey] = MainPivot.SelectedIndex;

      // Save current feeds from our managers
      foreach (SWSocialLinkManager manager in Managers)
        manager.SaveFeeds();
    }

    void fbManager_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      SWFacebookManager fbManager = sender as SWFacebookManager;
      System.Diagnostics.Debug.WriteLine(fbManager.Feeds[0].Count);
      HideLoadingBar();
    }

    void twitter_FeedsChanged(object sender, SocialLinkEventArgs e)
    {
      HideLoadingBar();
    }
 
    #region Appbar Commands

    void ShowAuthorizationView(object sender, EventArgs e)
    {
      this.NavigationService.Navigate(new Uri("/Views/AuthorizationView.xaml", UriKind.Relative));
    }

    void ShowSettingsView(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Refresh the feeds in the current pivot view
    /// Triggered by refresh button
    /// </summary>
    void RefreshCurrentLink(object sender, EventArgs e)
    {
      FetchFeedsForManager(MainPivot.SelectedIndex);
    }

    #endregion

    #region UI Related methods

    /// <summary>
    /// Display the loading progress bar with the specified message
    /// </summary>
    /// <param name="msg"></param>
    public void ShowLoadingBar(string msg)
    {
      loadingView.Visibility = Visibility.Visible;
      progressBar.IsIndeterminate = true;
      loadingText.Text = msg;
    }

    /// <summary>
    /// Hide the current progress bar
    /// </summary>
    public void HideLoadingBar()
    {
      loadingView.Visibility = Visibility.Collapsed;
      progressBar.IsIndeterminate = false;
    }

    #endregion

    private SWTwitterManager getTwitterManager()
    {
      foreach (SWSocialLinkManager man in Managers)
        if (man is SWTwitterManager)
          return (SWTwitterManager)man;
      return null;
    }

    private SWFacebookManager getFacebookManager()
    {
      foreach (SWSocialLinkManager man in Managers)
        if (man is SWFacebookManager)
          return (SWFacebookManager)man;
      return null;
    }

    /// <summary>
    /// Add a pivot item for this manager
    /// </summary>
    /// <param name="manager"></param>
    private void addPivotForManager(SWSocialLinkManager manager, UserControl fbc)
    {
      PivotItem fpi = new PivotItem();
      fpi.Header = manager.Title;
      fpi.Content = fbc;
      fbc.DataContext = manager;
      MainPivot.Items.Add(fpi);
      PivotContents.Add(fbc);
    }

    /// <summary>
    /// Called when we navigate to a new social link
    /// </summary>
    private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      // Update empty social links to see if new content.
      if (Managers[MainPivot.SelectedIndex].FeedCount == 0)
        FetchFeedsForManager(MainPivot.SelectedIndex);
    }

    /// <summary>
    /// Helper function to start fetching feeds for a manager
    /// </summary>
    /// <param name="index"></param>
    private void FetchFeedsForManager(int index)
    {
      if (!Managers[index].IsUpdating)
      {
        ShowLoadingBar("Loading " + Managers[index].Title + " Feeds...");
        Managers[index].FetchFeeds();
      }
    }
  }
}