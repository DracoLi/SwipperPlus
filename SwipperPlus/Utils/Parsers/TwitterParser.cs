using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using TweetSharp;
using SwipperPlus.Model.Twitter;
using SwipperPlus.Model;

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

      #region Adding tags in Message
      
      result.MessageTags = new List<SWTag>();
      TwitterEntities entities = tweet.Entities;
      if (result.IsRetweet) 
        entities = tweet.RetweetedStatus.Entities;

      // Add url tags
      if (entities.Urls != null &&
        entities.Urls.Count > 0)
      {
        foreach (TwitterUrl url in entities.Urls)
        {
          SWTag tag = new SWTag();
          tag.Type = SWTag.TagType.Link;
          tag.DisplayValue = url.Value;
          tag.Length = url.EndIndex - url.StartIndex;
          tag.Offset = url.StartIndex;
          result.MessageTags.Add(tag);
        }
      }

      // Add mentions
      if (entities.Mentions != null &&
        entities.Mentions.Count > 0)
      {
        foreach (TwitterMention one in entities.Mentions)
        {
          SWTag tag = new SWTag();
          tag.Type = SWTag.TagType.Mention;
          tag.DisplayValue = "@" + one.ScreenName;
          tag.Length = one.EndIndex - one.StartIndex;
          tag.Offset = one.StartIndex;
          result.MessageTags.Add(tag);
        }
      }

      // Add hashtags
      if (entities.HashTags != null &&
        entities.HashTags.Count > 0)
      {
        foreach (TwitterHashTag one in entities.HashTags)
        {
          SWTag tag = new SWTag();
          tag.Type = SWTag.TagType.Hashtag;
          tag.DisplayValue = "#" + one.Text;
          tag.Length = one.EndIndex - one.StartIndex;
          tag.Offset = one.StartIndex;
          result.MessageTags.Add(tag);
        }
      }

      // Sort all tags in desending offset order to aid replacement
      if (result.MessageTags.Count > 0)
        result.MessageTags = result.MessageTags.OrderByDescending(x => x.Offset).ToList();
      
      #endregion

      result.XmlMessage = RichTextBoxParser.ParseStringToXamlWithTags(result.Message, 
        result.MessageTags, isTwitter: true);

      // Add an photo if exists
      if (tweet.Entities.Media != null &&
        tweet.Entities.Media.Count > 0 &&
        tweet.Entities.Media[0].MediaType == TwitterMediaType.Photo)
      {
        TwitterMedia media = tweet.Entities.Media[0];
        result.PhotoUrl = new Uri(media.MediaUrl);
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
