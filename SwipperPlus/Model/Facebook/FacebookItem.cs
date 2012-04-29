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
    public IList<UInt64> FriendLikes { get; set; }

    public int CommentsCount { get; set; }
    public IList<FacebookComment> Comments { get; set; }
  }
}
