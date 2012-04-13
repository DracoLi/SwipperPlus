using System;
using System.Collections.Generic;
using SwipperPlus.Models;

namespace SwipperPlus.ViewModels
{
  public abstract class SWSocialLinkManager
  {
    public event EventHandler<SocialLinkEventArgs> FeedsChanged;

    public abstract bool HasValidAccessToken();

    // Fetch all feeds from this social link
    public abstract void FetchFeeds();

    // Save current feeds to persistant storange
    public abstract void SaveFeeds();

    public abstract bool HaveSavedFeeds();

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
    public SocialLinkEventArgs(string errorMsg)
    {
      if (!String.IsNullOrEmpty(errorMsg))
      {
        error = new Exception(errorMsg);
      }
    }
  }
}
