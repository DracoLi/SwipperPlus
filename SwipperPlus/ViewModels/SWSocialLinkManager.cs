using System;
using System.Collections.Generic;
using System.Windows.Controls;
using SwipperPlus.Model;

namespace SwipperPlus.ViewModel
{
  public enum FeedAction { New, More }

  public abstract class SWSocialLinkManager
  {
    public event EventHandler<SocialLinkEventArgs> FeedsChanged;

    public abstract string Title { get; }

    public FeedAction CurrentAction { get; set; }

    public bool IsUpdating { get; set; }

    public DateTime LastUpdated { get; set; }

    public abstract int FeedCount { get; }

    /// <summary>
    /// Fetch all feeds from this social link
    /// </summary>
    public abstract void FetchFeeds();

    /// <summary>
    /// Fetch some more feeds
    /// </summary>
    public abstract void GetMoreFeeds();

    /// <summary>
    /// Save current feeds to persistant storange
    /// </summary>
    public abstract void SaveFeeds();

    /// <summary>
    /// Check if we have any saved feeds
    /// </summary>
    public abstract bool HaveSavedFeeds();

    /// <summary>
    /// Load saved feeds from storage
    /// </summary>
    public abstract void LoadSavedFeeds();

    /// <summary>
    /// Called whenever we want to raise an event
    /// </summary>
    protected virtual void OnRaiseFeedsEvent(SocialLinkEventArgs e)
    {
      if (this.FeedsChanged != null)
      {
        this.FeedsChanged(this, e);
      }
    }
  }

  /// <summary>
  /// Fires after new feeds are obtained. If something with feeds, Error property will not be null
  /// </summary>
  public class SocialLinkEventArgs : EventArgs
  {
    private Exception error;
    public Exception Error
    {
      get { return error; }
    }

    public readonly FeedAction Action;

    /// <summary>
    /// A invalid callback instantiates an error
    /// </summary>
    /// <param name="errorMsg">The error message</param>
    public SocialLinkEventArgs(string errorMsg)
    {
      if (!String.IsNullOrEmpty(errorMsg))
        error = new Exception(errorMsg);
    }

    /// <summary>
    /// A valid callback specified feed status
    /// </summary>
    public SocialLinkEventArgs(FeedAction action)
    {
      Action = action;
    }
  }
}
