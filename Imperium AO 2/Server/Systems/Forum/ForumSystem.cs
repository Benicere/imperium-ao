using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using System.Linq;

namespace ImperiumAO.Server.Systems.Forum;

public class ForumSystem : IForumSystem
{
    private readonly Dictionary<int, ForumPost> _posts = new();
    private readonly Dictionary<int, ForumReply> _replies = new();
    private readonly Dictionary<string, ForumCategory> _categories = new();
    private readonly ILogger<ForumSystem> _logger;
    private int _postIdCounter = 1;
    private int _replyIdCounter = 1;

    public ForumSystem(ILogger<ForumSystem> logger)
    {
        _logger = logger;
        InitializeCategories();
    }

    private void InitializeCategories()
    {
        _categories["General"] = new() { Name = "General", Description = "General Discussion" };
        _categories["Updates"] = new() { Name = "Updates", Description = "Game Updates" };
        _categories["Events"] = new() { Name = "Events", Description = "Event Announcements" };
        _categories["Trading"] = new() { Name = "Trading", Description = "Trading Posts" };
    }

    public void CreatePost(ForumPost post)
    {
        post.Id = _postIdCounter++;
        post.PostedAt = DateTime.UtcNow;
        _posts[post.Id] = post;

        if (_categories.TryGetValue(post.Category, out var category))
        {
            category.PostCount++;
            category.LastPostAt = DateTime.UtcNow;
        }

        _logger.LogInformation($"Forum post {post.Id} created by {post.AuthorName}");
    }

    public void DeletePost(int postId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            _posts.Remove(postId);
            post.Replies.ForEach(r => _replies.Remove(r.Id));

            if (_categories.TryGetValue(post.Category, out var category))
            {
                category.PostCount = Math.Max(0, category.PostCount - 1);
            }

            _logger.LogInformation($"Forum post {postId} deleted");
        }
    }

    public void CreateReply(int postId, ForumReply reply)
    {
        if (!_posts.TryGetValue(postId, out var post))
        {
            return;
        }

        reply.Id = _replyIdCounter++;
        reply.PostedAt = DateTime.UtcNow;
        reply.PostId = postId;

        _replies[reply.Id] = reply;
        post.Replies.Add(reply);

        if (_categories.TryGetValue(post.Category, out var category))
        {
            category.ReplyCount++;
        }

        _logger.LogInformation($"Reply {reply.Id} created on post {postId}");
    }

    public void DeleteReply(int replyId)
    {
        if (_replies.TryGetValue(replyId, out var reply))
        {
            _replies.Remove(replyId);

            var post = _posts.Values.FirstOrDefault(p => p.Replies.Any(r => r.Id == replyId));
            if (post != null)
            {
                post.Replies.RemoveAll(r => r.Id == replyId);
            }

            _logger.LogInformation($"Reply {replyId} deleted");
        }
    }

    public ForumPost? GetPost(int postId)
    {
        _posts.TryGetValue(postId, out var post);
        return post;
    }

    public List<ForumPost> GetPostsByCategory(string category)
    {
        return _posts.Values.Where(p => p.Category == category).ToList();
    }

    public List<ForumPost> GetRecentPosts(int count)
    {
        return _posts.Values.OrderByDescending(p => p.PostedAt).Take(count).ToList();
    }

    public void LikePost(int postId, int playerId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            post.Likes++;
            _logger.LogInformation($"Post {postId} liked");
        }
    }

    public void DislikePost(int postId, int playerId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            post.Dislikes++;
            _logger.LogInformation($"Post {postId} disliked");
        }
    }

    public void LockPost(int postId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            post.IsLocked = true;
            _logger.LogInformation($"Post {postId} locked");
        }
    }

    public void UnlockPost(int postId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            post.IsLocked = false;
            _logger.LogInformation($"Post {postId} unlocked");
        }
    }

    public void StickyPost(int postId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            post.IsSticky = true;
            _logger.LogInformation($"Post {postId} stickied");
        }
    }

    public void UnstickyPost(int postId)
    {
        if (_posts.TryGetValue(postId, out var post))
        {
            post.IsSticky = false;
            _logger.LogInformation($"Post {postId} unstickied");
        }
    }
}


