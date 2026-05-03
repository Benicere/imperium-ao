using System;

namespace ImperiumAO.Common.Database.Models;

public record Account(
    int Id,
    string Username,
    string Email,
    string PasswordHash,
    string Salt,
    DateTime DateCreated,
    string? LastIp,
    DateTime DateLastLogin,
    int Credits,
    byte Status
);
