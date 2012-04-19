using System;
using System.Net;
using System.Collections.Generic;
using SwipperPlus.Model;
using SwipperPlus.Settings;

namespace SwipperPlus.Model
{
  public class SWLinkedInManager : SWSocialLinkManager
  {

    public SWLinkedInManager()
    {
    
    }

    public static bool IsConnected()
    {
      return SWTwitterSettings.IsConnected();
    }

    public override void FetchFeeds()
    {
      throw new NotImplementedException();
    }

    public override void UpdateFeeds()
    {
      throw new NotImplementedException();
    }

    public override void SaveFeeds()
    {
      throw new NotImplementedException();
    }

    public override bool HaveSavedFeeds()
    {
      throw new NotImplementedException();
    }

    public override void LoadSavedFeeds()
    {
      throw new NotImplementedException();
    }
  }
}
