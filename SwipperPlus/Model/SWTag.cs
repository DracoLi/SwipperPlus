using System;
using System.Windows;

namespace SwipperPlus.Model
{
  public class SWTag
  {
    public enum TagType { Page }

    public ulong ID { get; set; }

    /// <summary>
    /// The words of the tag
    /// </summary>
    public string DisplayValue { get; set; }

    /// <summary>
    /// Offset of tag in the sentence
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// How long the tag is
    /// </summary>
    public int Length { get; set; }

    public string Type { get; set; }
  }
}
