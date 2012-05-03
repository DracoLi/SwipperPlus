using System;
using System.Windows;
using System.Windows.Media;
using System.Web;
using System.Security;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using SwipperPlus.Model;

namespace SwipperPlus.Utils.Parsers
{
  public static class RichTextBoxParser
  {
    private const string beginning = "<Section xml:space=\"preserve\" HasTrailingParagraphBreakOnPaste=\"False\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">";
    private const string end = "</Section>";
    private const string urlregex = @"((ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_=]*)?)";

    /// <summary>
    /// Returns defaults styles for our richtextbox
    /// Styles are obtained from our main app resources
    /// </summary>
    private static IDictionary<string, string> GetDefaultStyles()
    {
      IDictionary<string, string> results = new Dictionary<string, string>();
      //ResourceDictionary currentResources = Application.Current.Resources;
      results["DefaultFeedSize"] = "18";// currentResources["DefaultFeedSize"].ToString();
      results["DefaultFeedFont"] = "Tomaho"; // currentResources["DefaultFeedFont"].ToString();
      results["DefaultFeedColor"] = "White"; // currentResources["DefaultFeedColor"].ToString();
      results["DeEmphasizeFeedColor"] = "#adafb3"; // currentResources["DeEmphasizeFeedColor"].ToString();
      results["EmphasizeFeedColor"] = "#5696d4"; // currentResources["EmphasizeFeedColor"].ToString();
      results["DefaultLinkColor"] = "#9fa1a5"; // currentResources["DefaultLinkColor"].ToString();
      results["DefaultMentionsColor"] = "#3277ba"; // currentResources["DefaultMentionsColor"].ToString();
      results["DefaultHashtagColor"] = "#adafb3"; // currentResources["DefaultHashtagColor"].ToString();
      results["DefaultDescriptionColor"] = "#d3d1d1";
      return results;
    }

    /// <summary>
    /// Parse the message into xaml string with tags
    /// </summary>
    /// <param name="msg">The message to parse</param>
    /// <param name="tags">Tags in the message</param>
    /// <param name="me">The tag that is the source person of the feed</param>
    /// <returns></returns>
    internal static string ParseStringToXamlWithTags(string msg, IList<SWTag> tags = null, bool isDescription = false, bool isTwitter = false)
    {
      IDictionary<string, string> styles = GetDefaultStyles();

      // Escape the text for an element attribute value.
      msg = escapeForAttributeValue(msg);

      // Replace all links from tags
      if (tags != null && tags.Count > 0)
      {
        foreach (SWTag tag in tags)
        {
          if (tag.Offset < 0) continue;

          string oneTag = msg.Substring(tag.Offset, tag.Length);

          // We only do explicit tags.
          // Tags that doesn't show anything is ignored.z
          if (tag.DisplayValue != oneTag) continue; 

          string replacement = null;
          if (tag.Type == SWTag.TagType.Link)
          {
            string navigationuri = GeneralUtils.UriForWebsiteNavigation(HttpUtility.UrlPathEncode(tag.DisplayValue)).ToString();
            navigationuri = navigationuri.Replace("&", "&amp;");
            replacement = "\" /><Hyperlink" + 
              makeAttribute("FontSize", (Int16.Parse(styles["DefaultFeedSize"])+.5).ToString()) +
              makeAttribute("FontFamily", styles["DefaultFeedFont"]) +
              makeAttribute("Foreground", styles["DefaultLinkColor"]) +
              makeAttribute("NavigateUri", navigationuri) +
              " >" + oneTag + "</Hyperlink><Run Text=\"";
          }
          else if (tag.Type == SWTag.TagType.Hashtag)
          {
            replacement = "\" /><Run" + makeAttribute("Foreground", styles["DefaultHashtagColor"]) +
              makeAttribute("Text", tag.DisplayValue) +
              " /><Run Text=\"";
          }
          else if (tag.Type == SWTag.TagType.Mention)
          {
            replacement = "\" /><Run" + makeAttribute("Foreground", styles["DefaultMentionsColor"]);

            // Adjust for self tag
            if (isDescription && tag.Offset == 0)
              replacement += makeAttribute("FontWeight", "Bold");

            // Write the tag
            replacement += makeAttribute("Text", tag.DisplayValue) + " /><Run Text=\"";
          }
          else
            throw new Exception("Tag not supported by RichTextBox exception");

          msg = msg.Substring(0, tag.Offset) + replacement + msg.Substring(tag.Offset + tag.Length);
        }
      }

      // Replace all links if not twitter
      if (!isTwitter)
      {
        string linkUri = GeneralUtils.UriForWebsiteNavigation("$1").ToString().Replace("&", "&amp;amp;");
        string linkReplacement = "\" /><Hyperlink " + makeAttribute("Foreground", styles["DefaultLinkColor"]) +
          " NavigateUri=\"" + linkUri + "\">$1</Hyperlink><Run Text=\"";
        Regex regex = new Regex(urlregex);
        msg = regex.Replace(msg, linkReplacement);
      }

      // Parse new lines and other things
      if (isDescription)
        styles["DefaultFeedColor"] = styles["DefaultDescriptionColor"];
      msg = commonParse(msg, ref styles);

      System.Diagnostics.Debug.WriteLine(beginning + msg + end);
      return beginning + msg + end;
    }

    /// <summary>
    /// Parse things that every xmal must have
    /// </summary>
    private static string commonParse(string msg, ref IDictionary<string, string> styles)
    {
      // Replace all newlines
      string newlineReplacement = "\" /><LineBreak /><Run Text=\"";
      msg = msg.Replace("\n", newlineReplacement);

      // Finish up our styled xaml
      msg = "<Run Text=\"" + msg + "\" />";
      msg = makeItParagraph(msg, ref styles);

      return msg;
    }

    private static string makeItParagraph(string elem, ref IDictionary<string, string> styles)
    {
      string result = "<Paragraph" +
        makeAttribute("FontSize", styles["DefaultFeedSize"]) +
        makeAttribute("FontFamily", styles["DefaultFeedFont"]) +
        makeAttribute("Foreground", styles["DefaultFeedColor"]);
      return result + ">" + elem + "</Paragraph>";
    }

    /// <summary>
    /// Escape string for the value of an xaml attribute
    /// </summary>
    private static string escapeForAttributeValue(string str)
    {
      return str.Replace("&", "&amp;amp;").Replace("<", "&lt;").Replace(">", "&gt;")
        .Replace("\\\"", "&quot;").Replace("\"", "&quot;");
    }

    /// <summary>
    /// Helper to generate attribute string
    /// </summary>
    /// <returns></returns>
    private static string makeAttribute(string name, string value)
    {
      return " " + name + "=\"" + value + "\"";
    }
  }
}
