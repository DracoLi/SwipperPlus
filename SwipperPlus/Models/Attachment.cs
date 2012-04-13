using System;
using System.Net;
using System.Windows;
using SwipperPlus.Utils;

namespace SwipperPlus.Models
{
  public class Attachment
  {
    public Uri Href { get; set; }
    public MediaType Type { get; set; }
    public string Name { get; set; }
    public Uri Icon { get; set; }
  }
}
