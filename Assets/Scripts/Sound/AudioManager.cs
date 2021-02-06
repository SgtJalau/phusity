using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        Play(SoundType.Theme);
    }

    public Sound FindSound(SoundType soundType)
    {
        return Array.Find(sounds, a => a.soundType == soundType);
    }

    public void Play(SoundType soundType)
    {
        FindSound(soundType)?.PlayRandomAudioFile();
    }

    public void Play(SoundType soundType, float pitch, float volume)
    {
        Sound s = FindSound(soundType);
        s?.PlayRandomAudioFile(pitch, volume);
    }
}