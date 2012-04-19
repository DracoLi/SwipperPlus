using System;
using System.Collections.Generic;
using SwipperPlus.Model;

namespace SwipperPlus.Model.Facebook
{
  /// <summary>
  /// Data that only a Facebook feed will have
  /// </summary>
  public class FacebookItem
  {
    public int LikesCount { get; set; }
    public List<UInt64> FriendLikes { get; set; }

    public int CommentsCount { get; set; }
    public List<FacebookComment> Comments { get; set; }
  }
}
