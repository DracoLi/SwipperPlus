using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// This represents a comment for a feed
  /// </summary>
  public class FacebookComment
  {
    public FacebookUser User;
    public string Message;
    public DateTime Date;

    /// <summary>
    /// Total likes
    /// </summary>
    public int Likes;

    /// <summary>
    /// Whether the user has liked this comment
    /// </summary>
    public bool UserLiked;
  }
}
