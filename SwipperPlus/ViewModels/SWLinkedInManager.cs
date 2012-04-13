using System;
using System.Net;
using System.Collections.Generic;
using SwipperPlus.Models;

namespace SwipperPlus.ViewModels
{
  public class SWLinkedInManager : SWSocialLinkManager
  {

    public SWLinkedInManager()
    {
    
    }

    public override bool HasValidAccessToken()
    {
      throw new NotImplementedException();
    }

    public override void FetchFeeds()
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
