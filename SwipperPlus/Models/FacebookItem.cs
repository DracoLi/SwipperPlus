using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using SwipperPlus.Models;

namespace SwipperPlus.Models
{
  public class FacebookItem
  {
    public int LikesCount { get; set; }
    public List<UInt64> FriendLikes { get; set; }

    public int CommentsCount { get; set; }
    public List<Comment> Comments { get; set; }
  }
}
