using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Forum;

public class ForumPost
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public bool IsSticky { get; set; }
    public bool IsLocked { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ForumReply> Replies { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

public class ForumReply
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Likes { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ForumCategory
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public int ReplyCount { get; set; }
    public DateTime LastPostAt { get; set; }
}

