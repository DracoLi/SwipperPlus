using System.Runtime.Serialization;

namespace SwipperPlus.Model
{
  [DataContract]
  public class SWToken
  {
    [DataMember]
    public string Key { get; set; }

    [DataMember]
    public string Secret { get; set; }

    public SWToken(string key, string sec)
    {
      Key = key;
      Secret = sec;
    }
  }
}
