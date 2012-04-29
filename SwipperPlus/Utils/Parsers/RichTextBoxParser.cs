using System;
using System.Windows;
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
    
    private static readonly IDictionary<string, string> defaultStyle = new Dictionary<string, string>()
    {
      {"FontSize", "20"},
      {"FontFamily", "Segoe WP"},
      {"Foreground", "#FFFFFFFF"},
      {"FontWeight", "Normal"},
      {"FontStyle", "Normal"},
      {"FontStretch", "Normal"},
      {"TextAlignment", "Left"}
    };

    private static readonly IDictionary<string, string> emphasizeStyle = new Dictionary<string, string>()
    {

    };

    private static readonly IDictionary<string, string> tagStyle = new Dictionary<string, string>()
    {
    
    };

    private static readonly IDictionary<string, string> linkStyle = new Dictionary<string, string>()
    {

    };

    /// <summary>
    /// Parse the message into a xmal string.
    /// The function tries to guess the links.
    /// </summary>
    internal static string ParseStringToXaml(string msg, IDictionary<string, string> style = null)
    {
      // Escape the text for an element attribute value.
      msg = escapeForAttributeValue(msg);
      
      // Parse new lines and other things
      msg = commonParse(msg, style);

      // Replace all links
      string linkReplacement = "\" /><Hyperlink NavigateUri=\"/Views/SWBrowser.xaml/type=link&amp;amp;value=$1\">$1</Hyperlink><Run Text=\"";
      Regex regex = new Regex(urlregex);
      msg = regex.Replace(msg, linkReplacement);

      return beginning + msg + end;
    }

    internal static string ParseStringToXamlWithTags(string msg, IList<SWTag> tags,
      IDictionary<string, string> style = null)
    {
      // Escape the text for an element attribute value.
      msg = escapeForAttributeValue(msg);

      // Replace all links from tags
      if (tags != null && tags.Count > 0)
      {
        foreach (SWTag tag in tags)
        {
          string oneTag = msg.Substring(tag.Offset, tag.Length);
          string replacement = "\" /><Hyperlink NavigateUri=\"/Views/SWBrowser.xaml/value=" +
            HttpUtility.UrlPathEncode(tag.DisplayValue) + "&amp;amp;type=" + tag.Type + " \">" + oneTag + "</Hyperlink><Run Text=\"";
          msg = msg.Substring(0, tag.Offset) + replacement + msg.Substring(tag.Offset + tag.Length);
        }
      }

      // Parse new lines and other things
      msg = commonParse(msg, style);

      return beginning + msg + end;
    }

    /// <summary>
    /// Parse things that every xmal must have
    /// </summary>
    private static string commonParse(string msg, IDictionary<string, string> style = null)
    {
      // Set default style
      style = style ?? defaultStyle;

      // Replace all newlines
      string newlineReplacement = "\" /><LineBreak /><Run Text=\"";
      msg = msg.Replace("\n", newlineReplacement);

      // Finish up our styled xaml
      msg = "<Run Text=\"" + msg + "\" />";
      msg = makeItParagraph(msg, style);

      return msg;
    }

    private static string makeItParagraph(string elem, IDictionary<string, string> attribs)
    {
      string result = "<Paragraph";
      if (attribs != null)
      {
        foreach (KeyValuePair<string, string> pair in attribs)
          result += " " + pair.Key + "=\"" + pair.Value + "\"";
      }
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
  }
}
