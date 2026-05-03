using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Forum;

public interface IForumSystem
{
    void CreatePost(ForumPost post);
    void DeletePost(int postId);
    void CreateReply(int postId, ForumReply reply);
    void DeleteReply(int replyId);
    ForumPost? GetPost(int postId);
    List<ForumPost> GetPostsByCategory(string category);
    List<ForumPost> GetRecentPosts(int count);
    void LikePost(int postId, int playerId);
    void DislikePost(int postId, int playerId);
    void LockPost(int postId);
    void UnlockPost(int postId);
    void StickyPost(int postId);
    void UnstickyPost(int postId);
}

