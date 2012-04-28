using System;
using System.Net;
using System.Collections.Generic;
using SwipperPlus.Model;
using SwipperPlus.Settings;

namespace SwipperPlus.ViewModel
{
  public class SWLinkedInManager : SWSocialLinkManager
  {

    public override string Title
    {
      get { return "LinkedIn"; }
    }

    public SWLinkedInManager()
    {
    
    }

    public static bool IsConnected()
    {
      return SWTwitterSettings.IsConnected();
    }

    public override void FetchFeeds()
    {
      /*
        if (!SWLinkedInManager.IsConnected())
      {
        System.Diagnostics.Debug.WriteLine("LinkedIn is not connected!");
        return;
      }

      if (linkedinManager == null) linkedinManager = new SWLinkedInManager();

      OAuthClient client = new OAuthClient(SWLinkedInSettings.ConsumerKey,
        SWLinkedInSettings.ConsumerSecret, SWLinkedInSettings.GetAccessToken())
      {
        Url = "http://api.linkedin.com/v1/people/~/network/updates",
        Parameters = { { "format", "json" } }
      };
      client.GetResponseText()
        .Subscribe(a =>
        {
          System.Diagnostics.Debug.WriteLine(a);
        });
       */
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
