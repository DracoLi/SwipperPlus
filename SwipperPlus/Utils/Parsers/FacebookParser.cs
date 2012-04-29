﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SwipperPlus.Model;
using SwipperPlus.Model.Facebook;
using SwipperPlus.Utils;

namespace SwipperPlus.Utils.Parsers
{
  public static class FacebookParser
  {
    /// <summary>
    /// Parses a Facebook feed
    /// </summary>
    public static FacebookFeed ParseFeed(JToken token, Dictionary<ulong, FacebookUser> people)
    {
      FacebookFeed result = new FacebookFeed();

      // Add info every feed has
      if (tokenHasValue(token["message"]))
      {
        result.Message = (string)token["message"];
        result.XmlMessage = RichTextBoxParser.ParseStringToXaml(result.Message);
        System.Diagnostics.Debug.WriteLine(result.XmlMessage);
      }
      result.ID = (string)token["post_id"];
      if (tokenHasValue(token["actor_id"]))
        result.SourceUser = people[(UInt64)token["actor_id"]];
      result.Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["created_time"]);

      //// Add Facebook properties ////
      FacebookItem fbItem = new FacebookItem();

      // Likes
      if (tokenHasValue(token["likes"]["count"]))
      {
        fbItem.LikesCount = (int)token["likes"]["count"];
        List<UInt64> flikes = new List<UInt64>();
        foreach (JToken person in token["likes"]["friends"].Children())
          flikes.Add((UInt64)person);
        if (flikes.Count > 0)
          fbItem.FriendLikes = flikes;
      }

      // Comments
      if (tokenHasValue(token["comments"]["count"]))
      {
        fbItem.CommentsCount = (int)token["comments"]["count"];
        List<FacebookComment> comments = new List<FacebookComment>();
        foreach (JToken comment in token["comments"]["comment_list"])
          comments.Add(ParseFacebookComment(comment));
        if (comments.Count > 0)
          fbItem.Comments = comments;
      }

      result.SocialProperties = fbItem;

      // Target person
      if (tokenHasValue(token["target_id"]))
        result.TargetUser = people[(UInt64)token["target_id"]];

      // Description + Description tags + xmal description
      if (tokenHasValue(token["description"]))
      {
        result.Description = (string)token["description"];
        if (tokenHasValue(token["description_tags"])) {
          result.DescriptionTags = new List<SWTag>();
          foreach (JToken tagToken in token["description_tags"].Values())
          {
            // Handle both types of description tags we going to get
            // Sometime its an object, others its an array
            JToken targetToken = tagToken;
            if (tagToken is JArray)
              targetToken = tagToken[0];
            
            SWTag tag = new SWTag
            {
              ID = (ulong)targetToken["id"],
              DisplayValue = (string)targetToken["name"],
              Offset = (int)targetToken["offset"],
              Length = (int)targetToken["length"],
              Type = (string)targetToken["type"]
            };
            result.DescriptionTags.Add(tag);
          }

          // Sort the tags in desending offset order to aid replacement
          if (result.DescriptionTags.Count > 0)
          {
            result.DescriptionTags = result.DescriptionTags.OrderByDescending(x => x.Offset).ToList();
          }
          result.XmlDescription = RichTextBoxParser.ParseStringToXamlWithTags(result.Description, result.DescriptionTags);
          System.Diagnostics.Debug.WriteLine(result.XmlDescription);
        }
      }

      // Attachment
      if (tokenHasValue(token["attachment"]) && token.HasValues)
      {
        FacebookAttachment fbAttach = new FacebookAttachment();
        JToken attachment = token["attachment"];

        // Figure out attachment type, href and name
        fbAttach.Type = FacebookAttachment.MediaType.Link; // Defaults to Link type
        if (tokenHasValue(attachment["href"]))
          fbAttach.Href = new Uri((string)attachment["href"]);
        if (tokenHasValue(attachment["name"]))
          fbAttach.Name = (string)attachment["name"];

        // Adjust values if we have some media in the attachment
        if (tokenHasValue(attachment["media"]) && attachment["media"].HasValues)
        {
          JToken media = attachment["media"][0];

          // Adjust type
          if (tokenHasValue(media["type"]))
          {
            string type = (string)media["type"];
            if (type.Equals("photo")) fbAttach.Type = FacebookAttachment.MediaType.Image;
            else if (type.Equals("video")) fbAttach.Type = FacebookAttachment.MediaType.Video;
            else if (type.Equals("link")) fbAttach.Type = FacebookAttachment.MediaType.Link;
          }

          // Add Src
          if (tokenHasValue(media["src"]))
            fbAttach.Icon = new Uri((string)media["src"]);

          // Use media href instead of attachment href
          if (tokenHasValue(media["href"]))
            fbAttach.Href = new Uri((string)media["href"]);
        }

        // Add this attachment to our feed if we have an href to attach
        if (fbAttach.Href != null)
          result.Attachment = fbAttach;
      }

      // Finally add the current feedtype
      result.FeedType = DetermineFacebookFeedType(result, (int)token["type"]);

      return result;
    }

    /// <summary>
    /// Parses a facebook person from a person JSON token
    /// </summary>
    public static FacebookUser ParseUser(JToken token)
    {
      return new FacebookUser((UInt64)token["id"])
      {
        DisplayName = (string)token["name"],
        Icon = new Uri((string)token["pic_square"])
      };
    }

    /// <summary>
    /// Parses a facebook comments from a json token
    /// </summary>
    public static FacebookComment ParseFacebookComment(JToken token)
    {
      return new FacebookComment()
      {
        Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["time"]),
        UserID = (UInt64)token["fromid"],
        Message = (string)token["text"],
        Likes = (int)token["likes"],
        UserLiked = (bool)token["user_likes"]
      };
    }

    /// <summary>
    /// Guess the type of the facebook feed from the content is has
    /// </summary>
    private static FeedType DetermineFacebookFeedType(FacebookFeed feed, int type)
    {
      FeedType result = FeedType.NotSupported;

      // Figure out type from supplied type param
      if      (type == 56 && feed.TargetUser != null)      result = FeedType.Conversation;
      else if ((type == 80 || type == 128 || type == 247) && 
                feed.Attachment != null)                  result = FeedType.Attachment;
      else if (type == 46 || feed.Message != null)        result = FeedType.Text;
      else                                                result = FeedType.Action;
      return result;
    }

    /// <summary>
    /// A helper that tells us whether a JToken has any value
    /// </summary>
    private static bool tokenHasValue(JToken token)
    {
      if (token == null) return false;
      return !String.IsNullOrEmpty(token.ToString());
    }
  }
}
