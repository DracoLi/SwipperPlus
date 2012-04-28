using System;
using System.Windows;
using System.Collections.Generic;
using TweetSharp;
using SwipperPlus.Model.Twitter;

namespace SwipperPlus.Utils.Parsers
{
  public static class TwitterParser
  {

    /// <summary>
    /// Parses a crappy tweet to our own tweet object
    /// </summary>
    /// <param name="tweet"></param>
    /// <returns></returns>
    public static SWTwitterFeed ParseTweet(TwitterStatus tweet)
    {
      SWTwitterFeed result = new SWTwitterFeed();

      // General info
      result.IsRetweet = tweet.RetweetedStatus != null;
      result.Date = tweet.CreatedDate;
      result.ID = tweet.Id;

      // Set TweetedUser
      SWTwitterUser postUser = new SWTwitterUser
      {
        ID = tweet.User.Id,
        DisplayName = tweet.User.Name,
        Icon = new Uri(tweet.User.ProfileImageUrl)
      };
      result.TweetedUser = postUser;

      // Get retweet information (original user)
      if (result.IsRetweet)
      {
        result.Message = tweet.RetweetedStatus.Text;
        SWTwitterUser originalUser = new SWTwitterUser
        {
          ID = tweet.RetweetedStatus.User.Id,
          DisplayName = tweet.RetweetedStatus.User.Name,
          Icon = new Uri(tweet.RetweetedStatus.User.ProfileImageUrl)
        };
        result.OriginalUser = originalUser;
      }
      else
      {
        result.OriginalUser = result.TweetedUser;
        result.Message = tweet.Text;
      }
      result.XmlMessage = RichTextBoxParser.ParseStringToRichTextBox(result.Message);

      // Get attached urls
      if (tweet.Entities.Urls != null && tweet.Entities.Urls.Count > 0)
      {
        result.Urls = new List<TwitterUrl>(tweet.Entities.Urls.Count);
        foreach (TwitterUrl url in tweet.Entities.Urls)
          result.Urls.Add(url);
      }

      return result;
    }

    /// <summary>
    /// Parse a DateTime into twitter time format
    /// </summary>
    internal static string ParseTwitterDate(DateTime time)
    {
      string result = "now";

      TimeSpan diff = time.Subtract(DateTime.Now);

      if (diff.TotalDays >= 1)
      {
        result = time.ToString("MMM dd");
      }
      else if (diff.TotalHours >= 1)
      {
        result = ((int)diff.TotalHours).ToString() + "h";
      }
      else if (diff.TotalMinutes >= 1)
      {
        result = ((int)diff.TotalMinutes).ToString() + "m";
      }
      
      return result;
    }
  }
}
