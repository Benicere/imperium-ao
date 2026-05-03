using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Audio;

public enum AudioType
{
    Music,
    SoundEffect,
    Ambient,
    Voice
}

public class AudioClip
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public AudioType Type { get; set; }
    public float Volume { get; set; } = 1.0f;
    public int DurationMs { get; set; }
    public bool IsLooping { get; set; }
    public DateTime StartedAt { get; set; }
    public bool IsPlaying { get; set; }

    public bool IsFinished => IsPlaying && (DateTime.UtcNow - StartedAt).TotalMilliseconds >= DurationMs;
}

public class MusicTrack
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public int DurationMs { get; set; }
    public float DefaultVolume { get; set; } = 0.7f;
    public bool IsLooping { get; set; }
    public string Zone { get; set; } = string.Empty;
}

