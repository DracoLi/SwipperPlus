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

namespace SwipperPlus.Models
{
  public class Comment
  {
    public string Message;
    public UInt64 UserID;
    public DateTime Date;
    public int Likes;
    public bool UserLiked;
  }
}
