using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SwipperPlus.Models;
using SwipperPlus.Utils;

namespace SwipperPlus.Utils
{
  public class FacebookParser
  {
    /// <summary>
    /// Parses a Facebook feed
    /// </summary>
    public static SWFacebookFeed ParseFeed(JToken token)
    {
      SWFacebookFeed result = new SWFacebookFeed();

      // Add info every feed has
      result.Message = TokenHasValue(token["message"]) ? (string)token["message"] : null;
      result.ID = (string)token["post_id"];
      if (TokenHasValue(token["actor_id"]))
        result.SourcePerson = (UInt64)token["actor_id"];
      result.Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["created_time"]);

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
        List<FacebookComment> comments = new List<FacebookComment>();
        foreach (JToken comment in token["comments"]["comment_list"])
          comments.Add(ParseFacebookComment(comment));
        if (comments.Count > 0)
          fbItem.Comments = comments;
      }

      result.FacebookProperties = fbItem;

      // Target person
      if (TokenHasValue(token["target_id"]))
        result.TargetPerson = (UInt64)token["target_id"];

      // Description
      if (TokenHasValue(token["description"]))
        result.Description = (string)token["description"];

      // Attachment
      if (TokenHasValue(token["attachment"]) && token.HasValues)
      {
        Attachment fbAttach = new Attachment();
        JToken attachment = token["attachment"];

        // Figure out attachment type, href and name
        fbAttach.Type = Attachment.MediaType.Link; // Defaults to Link type
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
            if (type.Equals("photo")) fbAttach.Type = Attachment.MediaType.Image;
            else if (type.Equals("video")) fbAttach.Type = Attachment.MediaType.Video;
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
      result.FeedType = DetermineFacebookFeedType(result);

      return result;
    }

    /// <summary>
    /// Parses a facebook person from a person JSON token
    /// </summary>
    public static FacebookUser ParseUser(JToken token)
    {
      return new FacebookUser(token["id"].ToString(), (string)token["name"],
        new Uri((string)token["pic_square"]));
    }

    /// <summary>
    /// Parses a facebook comments from a json token
    /// </summary>
    public static FacebookComment ParseFacebookComment(JToken token)
    {
      FacebookComment result = new FacebookComment()
      {
        Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["time"]),
        UserID = (UInt64)token["fromid"],
        Message = (string)token["text"],
        Likes = (int)token["likes"],
        UserLiked = (bool)token["user_likes"]
      };
      return result;
    }

    /// <summary>
    /// Guess the type of the facebook feed from the content is has
    /// </summary>
    private static FeedType DetermineFacebookFeedType(SWFacebookFeed feed)
    {
      FeedType result = FeedType.NotSupported;
      if (feed.AttachmentProperties != null) result = FeedType.Attachment;
      else if (feed.Description != null) result = FeedType.Action;
      else if (feed.TargetPerson != 0) result = FeedType.Conversation;
      else if (feed.Message != null) result = FeedType.Text;
      return result;
    }

    /// <summary>
    /// A helper that tells us whether a JToken has any value
    /// </summary>
    private static bool TokenHasValue(JToken token)
    {
      if (token == null) return false;
      return !String.IsNullOrEmpty(token.ToString());
    }
  }
}
