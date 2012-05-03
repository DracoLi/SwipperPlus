using System;
using System.Windows;
using System.Runtime.Serialization;

namespace SwipperPlus.Model.Twitter
{
  /// <summary>
  /// Represents a Twitter user
  /// </summary>
  [DataContract]
  public class SWTwitterUser
  {
    [DataMember]
    public int ID { get; set; }
    [DataMember]
    public string DisplayName { get; set; }
    [DataMember]
    public Uri Icon { get; set; }
  }
}
