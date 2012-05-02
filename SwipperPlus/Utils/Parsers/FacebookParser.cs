using System;
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

    private const int maxLinkLength = 120;

    /// <summary>
    /// Parses a Facebook feed
    /// </summary>
    public static FacebookFeed ParseFeed(JToken token, Dictionary<ulong, FacebookUser> people)
    {
      FacebookFeed result = new FacebookFeed();

      // Get id, time and actors (source and target)
      result.ID = (string)token["post_id"];
      result.Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["created_time"]);
      if (tokenHasValue(token["actor_id"]))
        result.SourceUser = people[(UInt64)token["actor_id"]];
      if (tokenHasValue(token["target_id"]))
        result.TargetUser = people[(UInt64)token["target_id"]];

      // Get the core message
      if (tokenHasValue(token["message"]))
      {
        // Get the message tags, null if no tags
        result.MessageTags = getTags(token["message_tags"]);

        // Format our message
        result.Message = removeTabs((string)token["message"]);
        result.XmlMessage = RichTextBoxParser.
          ParseStringToXamlWithTags(result.Message, result.MessageTags);

        result.HasMessage = true; // Used for binding message visibility
      }

      // Description + Description tags + xmal description
      if (tokenHasValue(token["description"]))
      {
        // Get description tags, null if no tags
        result.DescriptionTags = getTags(token["description_tags"]);

        // Format description
        result.Description = (string)token["description"];
        result.XmlDescription = RichTextBoxParser.
          ParseStringToXamlWithTags(result.Description, result.DescriptionTags, true);
      }

      #region Facebook Properties (Like, Comment, Share)
      
      FacebookItem fbItem = new FacebookItem();

      // Likes
      if (tokenHasValue(token["likes"]["count"]))
      {
        fbItem.LikesCount = (int)token["likes"]["count"];

        // Grab friend likes
        IList<FacebookUser> flikes = new List<FacebookUser>();
        foreach (JToken person in token["likes"]["friends"].Children())
        {
          FacebookUser likedUser = null;
          people.TryGetValue((ulong)person, out likedUser);
          if (likedUser != null)
            flikes.Add(people[(ulong)person]);
        }
        if (flikes.Count > 0)
          fbItem.FriendLikes = flikes;
      }

      // Comments
      if (tokenHasValue(token["comments"]["count"]))
      {
        fbItem.CommentsCount = (int)token["comments"]["count"];

        // Grab some comments
        List<FacebookComment> comments = new List<FacebookComment>();
        foreach (JToken comment in token["comments"]["comment_list"])
          comments.Add(ParseFacebookComment(comment, ref people));
        if (comments.Count > 0)
          fbItem.Comments = comments;
      }

      result.SocialProperties = fbItem;

      #endregion

      #region Facebook Attachment

      // Attachment
      if (tokenHasValue(token["attachment"]) && token.HasValues)
      {
        FacebookAttachment fbAttach = new FacebookAttachment();
        JToken attachment = token["attachment"];

        // Figure out attachment type, href and name
        fbAttach.Type = FacebookAttachment.MediaType.Link; // Defaults to Link type
        if (tokenHasValue(attachment["href"]))
          fbAttach.Source = new Uri((string)attachment["href"]);
        if (tokenHasValue(attachment["name"]))
          fbAttach.Name = removeTabs((string)attachment["name"]);

        // Adjust values if we have some media in the attachment
        if (tokenHasValue(attachment["media"]) && attachment["media"].HasValues)
        {
          // We only use the first media for now
          JToken media = attachment["media"][0];

          // Adjust type
          if (tokenHasValue(media["type"]))
          {
            string type = (string)media["type"];
            if (type.Equals("photo")) fbAttach.Type = FacebookAttachment.MediaType.Image;
            else if (type.Equals("link")) fbAttach.Type = FacebookAttachment.MediaType.Link;
            // TODO: Treat videos as links for now
            else if (type.Equals("video")) fbAttach.Type = FacebookAttachment.MediaType.Link;
            else fbAttach.Type = FacebookAttachment.MediaType.NotSupported;
          }

          // Add Src
          if (tokenHasValue(media["src"]))
          {
            fbAttach.Icon = new Uri((string)media["src"]);
          }
          
          // Use media href instead of attachment href
          if (tokenHasValue(media["href"]))
            fbAttach.Source = new Uri((string)media["href"]);

          // Add description if this attachment is a link
          if (fbAttach.Type == FacebookAttachment.MediaType.Link &&
            tokenHasValue(attachment["description"]))
            fbAttach.Description = GeneralUtils.TrimWords((string)attachment["description"], maxLinkLength);
        }

        // Add this attachment to our feed if the attachment is valid (has a source)
        if (fbAttach.Source != null)
          result.Attachment = fbAttach;
      }

      #endregion

      // Finally add the current feedtype
      if (token["typle"] != null)
        result.FeedType = DetermineFacebookFeedType(result, (int)token["type"]);

      // Ignore any type of feeds that we currently do not support
      if ((result.Attachment != null && result.Attachment.Type == FacebookAttachment.MediaType.NotSupported) ||
        (result.Message == null && result.Description == null && result.Attachment == null))
        return null;

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
    public static FacebookComment ParseFacebookComment(JToken token, ref Dictionary<ulong, FacebookUser> people)
    {
      FacebookComment fbc = new FacebookComment()
      {
        Date = GeneralUtils.UnixTimestampToDateTime((Int64)token["time"]),
        Message = (string)token["text"],
        Likes = (int)token["likes"],
        UserLiked = (bool)token["user_likes"]
      };
      people.TryGetValue(((ulong)token["fromid"]), out fbc.User);
      return fbc;
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

    /// <summary>
    /// Get the tags in a token that contains tags
    /// This is used to get message tags and description tags
    /// </summary>
    /// <param name="token">The description token</param>
    private static IList<SWTag> getTags(JToken token)
    {
      if (!tokenHasValue(token)) return null;

      IList<SWTag> result = new List<SWTag>();
      foreach (JToken tagToken in token.Values())
      {
        // Handle both types of description tags we going to get
        // Sometime its an object, others its an array
        if (tagToken is JArray)
          foreach (JToken innerToken in tagToken)
            result.Add(getTagFromToken(innerToken));
        else
          result.Add(getTagFromToken(tagToken));
      }

      // Sort the tags in desending offset order to aid replacement
      if (result.Count > 0)
        return result.OrderByDescending(x => x.Offset).ToList();
      return null;
    }

    /// <summary>
    /// A convenience method to return a tag token into a tag object
    /// </summary>
    private static SWTag getTagFromToken(JToken targetToken)
    {
      // Check if token is valid
      if (!tokenHasValue(targetToken)) return null;

      return new SWTag
      {
        ID = (ulong)targetToken["id"],
        DisplayValue = (string)targetToken["name"],
        Offset = (int)targetToken["offset"],
        Length = (int)targetToken["length"],
        Type = SWTag.GetType((string)targetToken["type"])
      };
    }

    /// <summary>
    /// Helper to remove \r in a string
    /// </summary>
    private static string removeTabs(string str)
    {
      return str.Replace("\\r", "");
    }
  }
}
