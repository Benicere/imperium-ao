using System;
using System.Collections.Generic;

namespace ImperiumAO.Server.Systems.Audio;

public interface IAudioSystem
{
    void PlaySound(string soundName, float volume = 1.0f);
    void PlayMusic(string musicName, bool loop = true);
    void StopMusic();
    void StopAllSounds();
    void SetVolume(float volume);
    void SetMusicVolume(float volume);
    void PlaySoundAt(string soundName, int x, int y, float volume = 1.0f);
    AudioClip? GetCurrentMusic();
    List<AudioClip> GetPlayingSounds();
    void RegisterAudioClip(AudioClip clip);
    void RegisterMusicTrack(MusicTrack track);
    MusicTrack? GetMusicForZone(string zone);
}

