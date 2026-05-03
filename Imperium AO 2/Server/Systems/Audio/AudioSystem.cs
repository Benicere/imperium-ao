using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImperiumAO.Server.Systems.Audio;

public class AudioSystem : IAudioSystem
{
    private readonly Dictionary<string, AudioClip> _soundLibrary = new();
    private readonly Dictionary<string, MusicTrack> _musicLibrary = new();
    private readonly List<AudioClip> _playingSounds = new();
    private AudioClip? _currentMusic;
    private float _masterVolume = 1.0f;
    private float _musicVolume = 0.7f;
    private readonly ILogger<AudioSystem> _logger;
    private int _clipIdCounter = 1;

    public AudioSystem(ILogger<AudioSystem> logger)
    {
        _logger = logger;
        InitializeAudio();
    }

    private void InitializeAudio()
    {
        _soundLibrary["attack"] = new() { Id = 1, Name = "attack", FilePath = "Sounds/attack.wav", Type = AudioType.SoundEffect, DurationMs = 500 };
        _soundLibrary["heal"] = new() { Id = 2, Name = "heal", FilePath = "Sounds/heal.wav", Type = AudioType.SoundEffect, DurationMs = 800 };
        _soundLibrary["levelup"] = new() { Id = 3, Name = "levelup", FilePath = "Sounds/levelup.wav", Type = AudioType.SoundEffect, DurationMs = 2000 };

        _musicLibrary["village"] = new() { Id = 1, Name = "village", FilePath = "Music/village.ogg", Zone = "Village", DurationMs = 180000, IsLooping = true };
        _musicLibrary["forest"] = new() { Id = 2, Name = "forest", FilePath = "Music/forest.ogg", Zone = "Forest", DurationMs = 240000, IsLooping = true };
        _musicLibrary["dungeon"] = new() { Id = 3, Name = "dungeon", FilePath = "Music/dungeon.ogg", Zone = "Dungeon", DurationMs = 200000, IsLooping = true };
    }

    public void PlaySound(string soundName, float volume = 1.0f)
    {
        if (_soundLibrary.TryGetValue(soundName, out var template))
        {
            var clip = new AudioClip
            {
                Id = _clipIdCounter++,
                Name = template.Name,
                FilePath = template.FilePath,
                Type = template.Type,
                Volume = volume * _masterVolume,
                DurationMs = template.DurationMs,
                IsLooping = false,
                StartedAt = DateTime.UtcNow,
                IsPlaying = true
            };

            _playingSounds.Add(clip);
            _logger.LogInformation($"Playing sound: {soundName}");
        }
    }

    public void PlayMusic(string musicName, bool loop = true)
    {
        if (_musicLibrary.TryGetValue(musicName, out var track))
        {
            StopMusic();

            _currentMusic = new AudioClip
            {
                Id = _clipIdCounter++,
                Name = track.Name,
                FilePath = track.FilePath,
                Type = AudioType.Music,
                Volume = _musicVolume,
                DurationMs = track.DurationMs,
                IsLooping = loop,
                StartedAt = DateTime.UtcNow,
                IsPlaying = true
            };

            _logger.LogInformation($"Playing music: {musicName}");
        }
    }

    public void StopMusic()
    {
        if (_currentMusic != null)
        {
            _currentMusic.IsPlaying = false;
            _logger.LogInformation($"Stopped music: {_currentMusic.Name}");
            _currentMusic = null;
        }
    }

    public void StopAllSounds()
    {
        foreach (var sound in _playingSounds)
        {
            sound.IsPlaying = false;
        }
        _playingSounds.Clear();
        StopMusic();
        _logger.LogInformation("All sounds stopped");
    }

    public void SetVolume(float volume)
    {
        _masterVolume = Math.Clamp(volume, 0f, 1f);
        foreach (var sound in _playingSounds)
        {
            sound.Volume = _masterVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Math.Clamp(volume, 0f, 1f);
        if (_currentMusic != null)
        {
            _currentMusic.Volume = _musicVolume;
        }
    }

    public void PlaySoundAt(string soundName, int x, int y, float volume = 1.0f)
    {
        PlaySound(soundName, volume * 0.5f);
        _logger.LogInformation($"Playing sound at ({x}, {y}): {soundName}");
    }

    public AudioClip? GetCurrentMusic()
    {
        return _currentMusic;
    }

    public List<AudioClip> GetPlayingSounds()
    {
        _playingSounds.RemoveAll(s => s.IsFinished);
        return _playingSounds;
    }

    public void RegisterAudioClip(AudioClip clip)
    {
        _soundLibrary[clip.Name] = clip;
        _logger.LogInformation($"Registered audio clip: {clip.Name}");
    }

    public void RegisterMusicTrack(MusicTrack track)
    {
        _musicLibrary[track.Name] = track;
        _logger.LogInformation($"Registered music track: {track.Name}");
    }

    public MusicTrack? GetMusicForZone(string zone)
    {
        return _musicLibrary.Values.FirstOrDefault(m => m.Zone == zone);
    }
}


