using System;
using System.Collections.Generic;

namespace ImperiumAO.Common.Entities;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLogin { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
