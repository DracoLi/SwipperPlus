using System;
using System.Collections.Generic;
using SwipperPlus.Model;

namespace SwipperPlus.Model
{
  public enum FeedStatus { New, Updated, Deleted }

  public abstract class SWSocialLinkManager
  {
    public event EventHandler<SocialLinkEventArgs> FeedsChanged;

    /// <summary>
    /// Fetch all feeds from this social link
    /// </summary>
    public abstract void FetchFeeds();

    /// <summary>
    /// Update current feeds by fetching only newer feeds from server
    /// </summary>
    public abstract void UpdateFeeds();

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

    public readonly FeedStatus Status;

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
    public SocialLinkEventArgs(FeedStatus status)
    {
      Status = status;
    }
  }
}
