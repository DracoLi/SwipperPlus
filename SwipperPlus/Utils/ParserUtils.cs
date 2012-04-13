using System;
using System.Net;
using System.Windows;
using Newtonsoft.Json.Linq;
using SwipperPlus.Models;
using System.Collections.Generic;

namespace SwipperPlus.Utils
{
  public static class ParserUtils
  {
    public static SWFeed ParseFeed(JToken token, SocialLinkType type)
    {
      SWFeed p = null;
      switch (type)
      {
        case SocialLinkType.Facebook:
          p = ParseFacebookFeed(token);
          break;
        case SocialLinkType.Twitter:
          p = ParseTwitterFeed(token);
          break;
      }
      return p;
    }

    /// <summary>
    /// Parse a person from a token
    /// </summary>
    /// <param name="token">The json token</param>
    /// <param name="type">The type of feed this is</param>
    /// <returns>A person object</returns>
    public static Person ParsePerson(JToken token, SocialLinkType type)
    {
      Person p = null;
      switch (type)
      {
        case SocialLinkType.Facebook:
          p = ParseFacebookPerson(token);
          break;
        case SocialLinkType.Twitter:
          p = ParseTwitterPerson(token);
          break;
      }
      return p;
    }

    private static SWFeed ParseFacebookFeed(JToken token)
    {
      SWFeed result = new SWFeed();

      // Info every feed has
      result.Message = TokenHasValue(token["message"]) ? (string)token["message"] : null;
      result.ID = (string)token["post_id"];
      if (TokenHasValue(token["actor_id"]))
        result.SourcePerson = (UInt64)token["actor_id"];
      result.Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["created_time"]);
      result.LinkType = SocialLinkType.Facebook;
      
      //// Add Facebook properties ////
      FacebookItem fbItem = new FacebookItem();
      
      // Likes
      if (TokenHasValue(token["likes"]["count"]))
      {
        fbItem.LikesCount = (int)token["likes"]["count"];
        List<UInt64> flikes = new List<UInt64>();
        foreach (JToken person in token["likes"]["friends"].Children())
          flikes.Add((UInt64)person);
        if (flikes.Count > 0)
          fbItem.FriendLikes = flikes;
      }
     
      // Comments
      if (TokenHasValue(token["comments"]["count"]))
      {
        fbItem.CommentsCount = (int)token["comments"]["count"];
        List<Comment> comments = new List<Comment>();
        foreach (JToken comment in token["comments"]["comment_list"])
          comments.Add(ParseFacebookComment(comment));
        if (comments.Count > 0)
          fbItem.Comments = comments;
      }

      result.FacebookProperties = fbItem;

      // Target person
      if (TokenHasValue(token["target_id"]))
        result.TargetPerson = (UInt64)token["target_id"];
      else
        result.TargetPerson = 0;

      // Description
      if (TokenHasValue(token["description"]))
        result.Description = (string)token["description"];

      // Attachment
      if (TokenHasValue(token["attachment"]) && token.HasValues)
      {
        Attachment fbAttach = new Attachment();
        JToken attachment = token["attachment"];

        // Figure out attachment type, href and name
        fbAttach.Type = MediaType.Link; // Defaults to Link type
        if (TokenHasValue(attachment["href"]))
          fbAttach.Href = new Uri((string)attachment["href"]);
        fbAttach.Name = (string)attachment["name"];

        // Adjust values if we have some media in the attachment
        if (TokenHasValue(attachment["media"]) && attachment["media"].HasValues)
        {
          JToken media = attachment["media"][0];

          // Adjust type
          if (TokenHasValue(media["type"]))
          {
            string type = (string)media["type"];
            if (type.Equals("photo")) fbAttach.Type = MediaType.Image;
            else if (type.Equals("video")) fbAttach.Type = MediaType.Video;
          }

          // Add Src
          if (TokenHasValue(media["src"]))  
            fbAttach.Icon = new Uri((string)media["src"]);

          // User media href instead of attachment href
          if (TokenHasValue(media["href"]))
            fbAttach.Href = new Uri((string)media["href"]);
        }

        // Add this attachment to our feed if we have an href to attach
        if (fbAttach.Href != null)
          result.AttachmentProperties = fbAttach;
      }
      
      // Finally add the current feedtype
      result.FeedType = DetermineFeedType(result);

      return result;
    }

    private static Person ParseFacebookPerson(JToken token)
    {
      return new Person((UInt64)token["id"], (string)token["name"],
        new Uri(token["pic_square"].ToString()), SocialLinkType.Facebook);
    }

    public static Comment ParseFacebookComment(JToken token)
    {
      Comment result = new Comment()
      {
        Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["time"]),
        UserID = (UInt64)token["fromid"],
        Message = (string)token["text"],
        Likes = (int)token["likes"],
        UserLiked = (bool)token["user_likes"]
      };
      return result;
    }

    private static SWFeed ParseTwitterFeed(JToken token)
    {
      throw new NotImplementedException();
    }

    private static Person ParseTwitterPerson(JToken token)
    {
      throw new NotImplementedException();
    }

    private static FeedType DetermineFeedType(SWFeed feed)
    {
      FeedType result = FeedType.NotSupported;
      if (feed.AttachmentProperties != null) result = FeedType.Attachment;
      else if (feed.Description != null) result = FeedType.Action;
      else if (feed.TargetPerson != 0) result = FeedType.Conversation;
      else if (feed.Message != null) result = FeedType.Text;
      return result;
    }

    private static bool TokenHasValue(JToken token)
    {
      if (token == null) return false;
      return !String.IsNullOrEmpty(token.ToString());
    }

  }
}
