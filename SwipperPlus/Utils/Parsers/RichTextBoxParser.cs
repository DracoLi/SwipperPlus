using System;
using System.Windows;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text.RegularExpressions;

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
      /*
      <Paragraph FontSize=\"20\" FontFamily=\"Segoe WP\" Foreground=\"#FFFFFFFF\" FontWeight=\"Normal\" FontStyle=\"Normal\" FontStretch=\"Normal\" TextAlignment=\"Left\">";
      */

    /// <summary>
    /// Parse the message into a xmal string.
    /// The function tries to guess the links.
    /// </summary>
    internal static string ParseStringToRichTextBox(string msg, IDictionary<string, string> style = null)
    {
      // Set default style
      style = style ?? defaultStyle;

      // Escape all double quotes in mesage since it will mess up our xaml
      // There shouldn't be any double quotes in links so that is fine
      msg = escapeDoubleQuotes(msg);

      // Replace all newlines
      string newlineReplacement = "\" /><LineBreak /><Run Text=\"";
      msg = msg.Replace("\\n", newlineReplacement);

      // Replace all links
      string linkReplacement = "\" /><Hyperlink NavigateUri=\"/Views/SWBrowser.xaml/uri=$1\">$1</Hyperlink><Run Text=\"";
      Regex regex = new Regex(urlregex);
      msg = regex.Replace(msg, linkReplacement);
      
      // Finish up our styled xaml
      msg = "<Run Text=\"" + msg + "\" />";
      msg = makeItParagraph(msg, style);

      return beginning + msg + end;
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
    private static string escapeDoubleQuotes(string str)
    {
      str = str.Replace("\\\"", "&quot;"); // Replace \" with a quote for xmal
      str = str.Replace("\"", "&quot;"); // Replace plain " with a quote for xmal
      str = str.Replace("\\\\", "\\"); // Replace double \\ with since \ since xmal is okay with slashes
      return str;
    }
  }
}
